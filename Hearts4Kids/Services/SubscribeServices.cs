using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using Hearts4Kids.Models;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using Hearts4Kids.Controllers;

namespace Hearts4Kids.Services
{
    public class SubscribeServices
    {
        public static bool AddEmail(string email)
        {
            if (!new RegexUtilities().IsValidEmail(email))
            {
                return false;
            }
            if (!HttpContext.Current.User.Identity.IsAuthenticated) //otherwise we already have the email!
            {
                using (ApplicationUserManager userManager = Controllers.AccountController.GetApplicationUserManager())
                {
                    if (userManager.FindByEmail(email) == null)
                    {
                        try
                        {
                            using (var db = new Hearts4KidsEntities())
                            {
                                db.NewsletterSubscribers.Add(new NewsletterSubscriber { Email = email });
                                db.SaveChanges();
                            }
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {

                        }
                    }
                }

            }
            return true;
        }
        public static IEnumerable<string> GetAdminEmails()
        {
            using (var a = new ApplicationDbContext())
            {
                return (from u in GetAdminContacts(Domain.Admin,a)
                        select u.Email).ToList();
            }
        }
        public static IQueryable<ApplicationUser> GetAdminContacts(string roleName, ApplicationDbContext context)
        {
            return from role in context.Roles
                   where role.Name == roleName
                   from userRoles in role.Users
                   join user in context.Users
                   on userRoles.UserId equals user.Id
                   where user.EmailConfirmed == true
                     // && user.LockoutEndDateUtc < DateTime.UtcNow
                   select user;
        }
    }

    public class MemberDetailService
    {
        public static BiosViewModel GetBioDetails(int userId)
        {
            using (var db = new Hearts4KidsEntities())
            {
                return (from u in db.UserBios
                        where u.Id == userId
                        select new BiosViewModel
                        {
                            Name = u.FirstName + " " + u.Surname,
                            Biography = u.Bio,
                            BioPicUrl = u.BioPicUrl,
                            CitationDescription = u.CitationDescription
                        }).First();
            }
        }
        public static BioDetailsViewModel GetMemberDetails(int userId)
        {
            using (var db = new Hearts4KidsEntities())
            {
                return (from b in db.UserBios
                        where b.Id == userId
                        select new RegisterDetailsViewModel
                        {
                            Firstname = b.FirstName,
                            Surname = b.Surname,
                            CitationDescription = b.CitationDescription,
                            Profession = (Domain.Professions)b.Profession,
                            Team = (Domain.Teams)b.Team,
                            Trustee = b.Trustee,
                        }).FirstOrDefault();
            }
        }
        public static void UpdateMemberDetails(BioDetailsViewModel model, int userId, ModelStateDictionary modelState)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var details = db.UserBios.Find(userId);
                if (details==null)
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
                    db.Database.ExecuteSqlCommand("DELETE FROM [dbo].[NewsletterSubscribers] WHERE Email=@p0",model.Email);
                    details = new UserBio();
                    db.UserBios.Add(details);
                }
                details.Id = userId;
                details.FirstName = model.Firstname;
                details.Surname = model.Surname;
                details.CitationDescription = model.CitationDescription;
                details.Profession = (int)model.Profession;
                details.Team = (int)model.Team;
                details.Trustee = model.Trustee;
                db.SaveChanges();
            }
        }
        public static void UpdateBios(BiosViewModel model, int userId, ModelStateDictionary modelState)
        {
            using (var db = new Hearts4KidsEntities())
            {
                var details = db.UserBios.Find(userId);
                details.BioPicUrl = model.BioPicUrl;
                details.Bio = model.Biography;
                db.SaveChanges();
            }
        }
    }

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
}