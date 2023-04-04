using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NewsApp.Data.Contracts;
using NewsApp.Helpers;
using NewsApp.Models.DataModels;
using NewsApp.PageModels.Base;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class SignUpPageModel : BaseViewModel
    {
        #region Constructor

        public SignUpPageModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            ICommandPasswordVisibleClicked = new Command(() => PasswordVisibleClicked());
            ICommandSignUpClicked = new Command(async () => await SignUpClicked());
            ICommandSignInClicked = new Command(async () => await SignInClicked());
            ICommandTermsOfServicesClicked = new Command(async () => await TermsOfServicesClicked());
            ICommandPrivacyPolicyClicked = new Command(async () => await PrivacyPolicyClicked());
        }

        #endregion Constructor

        #region Private Variables

        private readonly IUserRepository _userRepository;
        private bool _isPassword = true;
        private string _passwordVisibilityIcon = "\uf070";
        private bool _isAgreeToTos;

        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        private string _validateResponse = string.Empty;

        #endregion Private Variables

        #region Public Properties

        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                if (_isPassword == value) return;
                _isPassword = value;
                RaisePropertyChanged();
            }
        }

        public string PasswordVisibilityIcon
        {
            get => _passwordVisibilityIcon;
            set
            {
                if (_passwordVisibilityIcon == value) return;
                _passwordVisibilityIcon = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAgreeToTOS
        {
            get => _isAgreeToTos;
            set
            {
                if (_isAgreeToTos == value) return;
                _isAgreeToTos = value;
                RaisePropertyChanged();
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName == value) return;
                _firstName = value;
                RaisePropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName == value) return;
                _lastName = value;
                RaisePropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email == value) return;
                _email = value;
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password == value) return;
                _password = value;
                RaisePropertyChanged();
            }
        }

        #endregion Public Properties

        #region ICommands

        public ICommand ICommandPasswordVisibleClicked { get; set; }
        public ICommand ICommandSignUpClicked { get; set; }
        public ICommand ICommandSignInClicked { get; set; }
        public ICommand ICommandTermsOfServicesClicked { get; set; }
        public ICommand ICommandPrivacyPolicyClicked { get; set; }

        #endregion ICommands

        #region Private Methods

        private void PasswordVisibleClicked()
        {
            if (IsPassword)
            {
                PasswordVisibilityIcon = "\uf06e"; //Open Eye
                IsPassword = false;
            }
            else
            {
                PasswordVisibilityIcon = "\uf070"; //Close Eye
                IsPassword = true;
            }
        }

        private async Task SignUpClicked()
        {
            IsBusy = true;

            if (string.IsNullOrEmpty(ValidateFields()))
            {
                if (IsAgreeToTOS)
                {
                    var isInserted = await _userRepository.SaveUserAync(new UserDataModel
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Password = Password,
                        CreatedDateTime = DateTime.Now
                    });

                    if (isInserted)
                        await CoreMethods.PopToRoot(true);
                    else
                        await CoreMethods.DisplayAlert("Alert", "User already exists", "Ok");
                }
                else
                {
                    await CoreMethods.DisplayAlert("Alert", "Please check & agree to the TOS / Privacy Policy", "Ok");
                }
            }
            else
            {
                await CoreMethods.DisplayAlert("Alert", _validateResponse, "Ok");
            }

            IsBusy = false;
        }

        private async Task SignInClicked()
        {
            await CoreMethods.PopToRoot(true);
        }

        private async Task TermsOfServicesClicked()
        {
            await CoreMethods.DisplayAlert("Alert", "TermsofServices Tapped", "Ok");
        }

        private async Task PrivacyPolicyClicked()
        {
            await CoreMethods.DisplayAlert("Alert", "Privacy Policy Tapped", "Ok");
        }

        private string ValidateFields()
        {
            _validateResponse = string.Empty;
            if (string.IsNullOrEmpty(FirstName) && FirstName.Length < 4)
            {
                _validateResponse = "First Name shouldn't be empty or should be greater than 3 letters";
                return _validateResponse;
            }

            if (string.IsNullOrEmpty(LastName) && LastName.Length < 4)
            {
                _validateResponse = "Last Name shouldn't be empty or should be greater than 3 letters";
                return _validateResponse;
            }

            if (string.IsNullOrEmpty(Email) && Email.Length < 4)
            {
                _validateResponse = "Email shouldn't be empty or should be greater than 3 letters";
                return _validateResponse;
            }

            if (!Validators.IsEmail(Email))
            {
                _validateResponse = "Email should be in correct format";
                return _validateResponse;
            }

            if (string.IsNullOrEmpty(Password) && Password.Length < 4)
            {
                _validateResponse = "Password shouldn't be empty or should be greater than 3 letters";
                return _validateResponse;
            }

            return _validateResponse;
        }

        #endregion Private Methods
    }
}