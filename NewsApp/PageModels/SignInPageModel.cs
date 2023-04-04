using System.Threading.Tasks;
using System.Windows.Input;
using FreshMvvm;
using NewsApp.Constants;
using NewsApp.Data.Contracts;
using NewsApp.Helpers;
using NewsApp.PageModels.Base;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NewsApp.PageModels
{
    public class SignInPageModel : BaseViewModel
    {
        public SignInPageModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            ICommandPasswordVisibleClicked = new Command(() =>  PasswordVisibleClicked());
            ICommandForgotPasswordClicked = new Command(async () => await ForgotPasswordClicked());
            ICommandLoginClicked = new Command(async () => await LoginClicked());
            ICommandSignUpClicked = new Command(async () => await SignUpClicked());
        }

        #region Private Variables

        private readonly IUserRepository _userRepository;
        private bool _isPassword = true;
        private string _passwordVisibilityIcon = "\uf070";
        private string _email = string.Empty;
        private string _password = string.Empty;

        #endregion Private Variables

        #region Public Properties

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

        #endregion Public Properties

        #region ICommands

        public ICommand ICommandPasswordVisibleClicked { get; set; }
        public ICommand ICommandForgotPasswordClicked { get; set; }
        public ICommand ICommandLoginClicked { get; set; }
        public ICommand ICommandSignUpClicked { get; set; }

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

        private async Task ForgotPasswordClicked()
        {
            await CoreMethods.PushPageModel<ForgotPasswordPageModel>();
        }

        private async Task LoginClicked()
        {
            IsBusy = true;

            if (ValidateFields())
            {
                var user = await _userRepository.GetUserAync(Email, Password);

                if (user != null)
                {
                    Preferences.Set(PreferencesKey.IsLoggedIn, true);
                    Preferences.Set(PreferencesKey.IsUserEmail, Email);
                    var page = FreshPageModelResolver.ResolvePageModel<HomePageModel>();
                    Application.Current.MainPage = new FreshNavigationContainer(page);
                }
                else
                {
                    await CoreMethods.DisplayAlert("Alert", "User does not exist, Please signup to proceed", "Ok");
                }
            }
            else
            {
                await CoreMethods.DisplayAlert("Alert", "Please check Email and the Password", "Ok");
            }

            IsBusy = false;
        }

        private async Task SignUpClicked()
        {
            await CoreMethods.PushPageModel<SignUpPageModel>();
        }

        private bool ValidateFields()
        {
            return !string.IsNullOrEmpty(Email) && Email.Length > 3 && Validators.IsEmail(Email) &&
                   !string.IsNullOrEmpty(Password) && Password.Length > 3;
        }

        #endregion Private Methods
    }
}