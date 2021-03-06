using System;
using System.Reactive;
using ReactiveUI;
using Serilog;
using UnitystationLauncher.Models;

namespace UnitystationLauncher.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly AuthManager authManager;
        private readonly Lazy<LoginViewModel> loginVM;
        string? email;
        string? password;
        string? username;
        private string creationMessage;
        private string endButtonText;
        private bool isFormVisible;
        private bool isWaitingVisible;
        private bool isCreatedVisible;
        private bool creationSuccess = false;

        public string? Username
        {
            get => username;
            set => this.RaiseAndSetIfChanged(ref username, value);
        }

        public string? Email
        {
            get => email;
            set => this.RaiseAndSetIfChanged(ref email, value);
        }

        public string? Password
        {
            get => password;
            set => this.RaiseAndSetIfChanged(ref password, value);
        }

        public bool IsFormVisible
        {
            get => isFormVisible;
            set => this.RaiseAndSetIfChanged(ref isFormVisible, value);
        }

        public bool IsCreatedVisible
        {
            get => isCreatedVisible;
            set => this.RaiseAndSetIfChanged(ref isCreatedVisible, value);
        }
        
        public bool IsWaitingVisible
        {
            get => isWaitingVisible;
            set => this.RaiseAndSetIfChanged(ref isWaitingVisible, value);
        }

        public string CreationMessage
        {
            get => creationMessage;
            set => this.RaiseAndSetIfChanged(ref creationMessage, value);
        }

        public string EndButtonText
        {
            get => endButtonText;
            set => this.RaiseAndSetIfChanged(ref endButtonText, value);
        }

        public ReactiveCommand<Unit, LoginViewModel?> Cancel { get; }
        public ReactiveCommand<Unit, LoginViewModel?> DoneButton { get; }
        public ReactiveCommand<Unit, Unit> Submit { get; }

        public SignUpViewModel(AuthManager authManager, Lazy<LoginViewModel> loginVM)
        {
            IsFormVisible = true;
            IsWaitingVisible = false;
            IsCreatedVisible = false;
            this.authManager = authManager;
            this.loginVM = loginVM;
            var possibleCredentials = this.WhenAnyValue(
                x => x.Email,
                x => x.Password,
                x => x.Username,
                (u, p, i) =>
                    !string.IsNullOrWhiteSpace(u) &&
                    !string.IsNullOrWhiteSpace(p) &&
                    p.Length > 6 &&
                    !string.IsNullOrEmpty(i));

            Submit = ReactiveCommand.Create(
                UserCreate, possibleCredentials);

            Cancel = ReactiveCommand.Create(ReturnToLogin);

            DoneButton = ReactiveCommand.Create(CreationEndButton);
        }

        public async void UserCreate()
        {
            IsFormVisible = false;
            creationSuccess = true;
            IsWaitingVisible = true;
            
            try
            { 
                var authLink = await authManager.CreateAccount(username, email, password);
            } catch (Exception e)
            {
                Log.Error(e, "Login failed");
                creationSuccess = false;
            }

            if (creationSuccess)
            {
                CreationMessage = $"Success! An email has been sent to \r\n{email}\r\n" +
                                  $"Please click the link in the email to verify\r\n" +
                                  $"your account before signing in.";
                EndButtonText = "Done";
            }
            else
            {
                CreationMessage = $"Something went wrong with the verification email server.\r\n" +
                    $"A reset password email has been sent to {email} as a work around.\r\n" +
                    $"Please reset your password and try to log in.";
                authManager.SendForgotPasswordEmail(email);
                EndButtonText = "Back";
            }
            
            IsWaitingVisible = false;
            IsCreatedVisible = true;
        }

        public LoginViewModel? ReturnToLogin()
        {
            return loginVM.Value;
        }
        
        public LoginViewModel? CreationEndButton()
        {
            return loginVM.Value;
        }
    }
}