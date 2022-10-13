using System;
using System.Collections.Generic;
using System.Linq;

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
            int? speakerId = null;

            RegisterError? someError = ValidateData();

            if (someError != null)
                return new RegisterResponse(someError);

            if (AppearsExceptional() is false && HasObviousRedFlags() is true)
                return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);

            if (Sessions.Count == 0)
                return new RegisterResponse(RegisterError.NoSessionsProvided);

            bool approvedSpeaker = false;

            foreach (var session in Sessions)
            {
                List<string> ot = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };

                foreach (var tech in ot)
                {
                    if (session.Title.Contains(tech) || session.Description.Contains(tech))
                    {
                        session.Approved = false;
                        break;
                    }
                    else
                    {
                        session.Approved = true;
                        approvedSpeaker = true;
                    }
                }
            }

            if (approvedSpeaker)
            {
                //if we got this far, the speaker is approved
                //let's go ahead and register him/her now.
                //First, let's calculate the registration fee.
                //More experienced speakers pay a lower fee.
                if (YearsSperience <= 1)
                {
                    RegistrationFee = 500;
                }
                else if (YearsSperience >= 2 && YearsSperience <= 3)
                {
                    RegistrationFee = 250;
                }
                else if (YearsSperience >= 4 && YearsSperience <= 5)
                {
                    RegistrationFee = 100;
                }
                else if (YearsSperience >= 6 && YearsSperience <= 9)
                {
                    RegistrationFee = 50;
                }
                else
                {
                    RegistrationFee = 0;
                }

                try
                {
                    speakerId = repository.SaveSpeaker(this);
                }
                catch (Exception e)
                {
                    //TODO: in case the db call fails
                }
            }
            else
            {
                return new RegisterResponse(RegisterError.NoSessionsApproved);
            }

            return new RegisterResponse((int)speakerId);
        }

        private RegisterError? ValidateData()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                return RegisterError.FirstNameRequired;

            if (string.IsNullOrWhiteSpace(LastName))
                return RegisterError.LastNameRequired;

            if (string.IsNullOrWhiteSpace(Email))
                return RegisterError.EmailRequired;

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
    }
}