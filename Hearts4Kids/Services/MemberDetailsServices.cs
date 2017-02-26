using Hearts4Kids.Domain;
using Hearts4Kids.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Hearts4Kids.Services
{
    public class MemberDetailService
    {
        private Hearts4KidsEntities db;

        public MemberDetailService(Hearts4KidsEntities db)
        {
            this.db = db;
        }

        public IEnumerable<BioSummaryModel> GetBioSummaries()
        {
            return (from u in db.UserBios
                    select new BioSummaryModel
                    {
                        Name = u.FirstName + " " + u.Surname,
                        CitationDescription = u.CitationDescription,
                        UserId = u.Id,
                        MainTeamPage = u.MainTeamPage,
                        Approved = u.Approved
                    }).ToList();
        }

        public BiosViewModel GetBioDetails(int userId)
        {
            return (from u in db.UserBios
                    where u.Id == userId
                    select new BiosViewModel
                    {
                        Name = u.FirstName + " " + u.Surname,
                        Biography = u.Bio,
                        BioPicUrl = u.BioPicUrl,
                        CitationDescription = u.CitationDescription,
                        UserId = userId,
                        MainTeamPage = u.MainTeamPage,
                        Approved = u.Approved
                    }).FirstOrDefault();
        }

        public BioDetailsViewModel GetMemberDetails(int userId)
        {
            return (from b in db.UserBios
                    where b.Id == userId
                    select new RegisterDetailsViewModel
                    {
                        Firstname = b.FirstName,
                        Surname = b.Surname,
                        CitationDescription = b.CitationDescription,
                        Profession = (DomainConstants.Professions)b.Profession,
                        Team = (DomainConstants.Teams)b.Team,
                        Trustee = b.Trustee,
                        UserId = userId
                    }).FirstOrDefault();
        }

        public bool BioRequired(int userId)
        {
            return (from u in db.UserBios
                    where u.Id == userId
                    select u.Bio).Any(b => b == null || b == string.Empty);
        }

        public void UpdateMemberDetails(BioDetailsViewModel model, ModelStateDictionary modelState)
        {
            var details = db.UserBios.Find(model.UserId);
            if (details == null)
            {
                // user being created - make sure they haven't subscribed
                /* Not sure why this isn't working, but the final savechanges (outside this block) 
                Creates DbUpdateConcurrencyException
                NewsletterSubscriber subDel = new NewsletterSubscriber { Email = model.Email };
                db.Entry(subDel).State = System.Data.Entity.EntityState.Deleted;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException) { }
                */
                db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[NewsletterSubscribers] WHERE Email=@p0", model.Email);
                details = new UserBio();
                db.UserBios.Add(details);
            }
            details.Id = model.UserId;
            details.FirstName = model.Firstname;
            details.Surname = model.Surname;
            details.CitationDescription = model.CitationDescription;
            details.Profession = model.Profession.Value;
            details.Team = model.Team.Value;
            details.Trustee = model.Trustee;
            db.SaveChanges();
        }

        public async Task<List<AllUserModel>> UserBioLength(string currentUser)
        {
            if (string.IsNullOrEmpty(currentUser)) { throw new ArgumentNullException("currentUser"); }
            return await (from u in db.AspNetUsers
                          select new AllUserModel
                          {
                              UserId = u.Id,
                              Email = u.Email,
                              UserName = u.UserName,
                              IsAdministrator = u.Roles.Any(r => r.Name == Domain.DomainConstants.Admin),
                              IsSelf = u.UserName == currentUser,
                              HasRegistered = u.PasswordHash != null,
                              BioLength = (u.UserBio == null || u.UserBio.Bio == null) ? 0 : u.UserBio.Bio.Length
                          }).ToListAsync();
        }

        public async Task<List<UserContacts>> GetAllUserContacts(string currentUser)
        {
            if (string.IsNullOrEmpty(currentUser)) { throw new ArgumentNullException("currentUser"); }
            var returnVar = await (from u in db.AspNetUsers
                                   where u.UserName != currentUser
                                   let b = u.UserBio

                                   select new UserContacts
                                   {
                                       Name = (b.FirstName == null) ? u.UserName : (b.FirstName + " " + b.Surname),
                                       Email = u.Email,
                                       Phone = u.PhoneNumber
                                   }).ToListAsync();
            /*
            returnVar.AddRange(await (from s in db.NewsletterSubscribers
                                        select new UserContacts
                                        {
                                            Name = s.Email + " (subscriber)",
                                            Email = s.Email
                                        }).ToListAsync());
            */
            return returnVar;
        }

        public async Task UpdateBios(BiosViewModel model, ModelStateDictionary modelState, bool isAdmin)
        {
            var san = new Ganss.XSS.HtmlSanitizer();
            model.Biography = san.Sanitize(model.Biography, HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)); //heavy op - do this before opening db connection
            var details = db.UserBios.Find(model.UserId);
            if (model.BioPicUrl != null)
            {
                details.BioPicUrl = model.BioPicUrl.Replace(" ", "%20");
            }

            details.Bio = model.Biography;
            details.MainTeamPage = model.MainTeamPage;
            details.Approved = isAdmin ? model.Approved :
                (details.Approved
                    && !db.ChangeTracker.Entries().Any(e => e.State == System.Data.Entity.EntityState.Modified));
            await db.SaveChangesAsync();
        }

        private Expression<Func<UserBio, BioDisplay>> GetBioConverter()
        {
            return b => new BioDisplay
            {
                Bio = b.Bio,
                BioPicUrl = b.BioPicUrl ?? defaultBioPic,
                CitationDescription = b.CitationDescription,
                Name = b.FirstName + " " + b.Surname,
                Profession = b.Profession,
                Team = b.Team,
                Trustee = b.Trustee
            };
        }

        public async Task<IEnumerable<BioDisplay>> GetStudents()
        {
            return await (db.UserBios.Where(b => b.Profession == DomainConstants.Professions.Student)
                            .Select(GetBioConverter())).ToListAsync();
        }

        public string defaultBioPic = "~/Content/Photos/Bios/Surgical.png";
        public async Task<Dictionary<DomainConstants.Teams, ILookup<DomainConstants.Professions, BioDisplay>>> GetBiosForDisplay(bool isMainPage)
        {
            var bios = await (db.UserBios.Where(b => b.MainTeamPage == isMainPage
                                                    && b.Approved
                                                    && b.Profession != DomainConstants.Professions.Student)
                .Select(GetBioConverter())).ToListAsync();

            return bios.GroupBy(b => b.Team)
                .ToDictionary(teamGroup => teamGroup.Key,
                                teamGroup => teamGroup.ToLookup(thing => thing.Profession));
            /*
            return (from person in bios
                    group person by person.Team into teams
                    from professions in
                        (from person in teams
                            group person by person.Profession)
                    group professions by teams.Key);
                    */
        }
    }
    /*
    Use the existing validator
    public class RegexUtilities
    {
        bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
                                        RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
    */
}