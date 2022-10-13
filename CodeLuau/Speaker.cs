using System;
using System.Collections.Generic;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace CodeLuau
{
    public class Speaker
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? YearsSperience { get; set; }
        public bool HasBlog { get; set; }
        public string BlogURL { get; set; }
        public WebBrowser Browser { get; set; }
        public List<string> Certifications { get; set; }
        public string Employer { get; set; }
        public int RegistrationFee { get; set; }
        public List<Session> Sessions { get; set; }

        public RegisterResponse Register(IRepository repository)
        {
            RegisterError? someError = ValidateRegistration();

            if (someError != null)
            {
                return new RegisterResponse(someError);
            }

            CalculateRegistrationFee();

            return new RegisterResponse(repository.SaveSpeaker(this));
        }

        private RegisterError? ValidateRegistration()
        {
            RegisterError? someError = ValidateData();

            if (someError != null)
                return someError;

            if (!AppearsExceptional() && HasObviousRedFlags())
                return RegisterError.SpeakerDoesNotMeetStandards;

            if (!ApproveSessions())
                return RegisterError.NoSessionsApproved;

            return default;
        }

        private RegisterError? ValidateData()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                return RegisterError.FirstNameRequired;

            if (string.IsNullOrWhiteSpace(LastName))
                return RegisterError.LastNameRequired;

            if (string.IsNullOrWhiteSpace(Email))
                return RegisterError.EmailRequired;

            if (!Sessions.Any())
                return RegisterError.NoSessionsProvided;

            return default;
        }

        private bool AppearsExceptional()
        {
            List<string> preferedEmployers = new List<string>() { "Pluralsight", "Microsoft", "Google" };

            return YearsSperience > 10
                || HasBlog
                || Certifications.Count > 3
                || preferedEmployers.Contains(Employer);
        }

        private bool HasObviousRedFlags()
        {
            var oldEmailDomains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

            string emailDomain = Email.Split('@').Last();

            return oldEmailDomains.Contains(emailDomain)
                || (Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9);
        }

        private bool ApproveSessions()
        {
            return Sessions.Any(session => SessionIsAboutOldTechnology(session) is false);
        }

        private bool SessionIsAboutOldTechnology(Session session)
        {
            List<string> oldTechnologies = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

            return oldTechnologies.Any(tech => session.Title.Contains(tech) || session.Description.Contains(tech));
        }

        private void CalculateRegistrationFee()
        {
            RegistrationFee = YearsSperience switch
            {
                int n when n <= 1 => 500,
                int n when n >= 2 && n <= 3 => 250,
                int n when n >= 4 && n <= 5 => 100,
                int n when n >= 6 && n <= 9 => 50,
                _ => 0,
            };
        }
    }
}