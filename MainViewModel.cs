using PopWithOAuth2Wpf.Services;
using PopWithOAuth2Wpf.Services.OAuth;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PopWithOAuth2Wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private string _LogMessage = "";
        public string LogMessage
        {
            get
            {
                return _LogMessage;
            }
            set
            {
                _LogMessage = value;
                NotifyPropertyChanged("LogMessage");
            }
        }


        private Pop3Service _Pop3;
        private GoogleOAuthService _GoogleOAuth;
        private ExchangeOAuthService _ExchangeOAuth;

        public ICommand GoogleCommand { get; private set; }
        public ICommand ExchangeCommand { get; private set; }


        public MainViewModel(Pop3Service pop3,
            GoogleOAuthService google,
            ExchangeOAuthService exchange)
        {
            _Pop3 = pop3;
            _GoogleOAuth = google;
            _ExchangeOAuth = exchange;

            GoogleCommand = new GoogleCommandImpl(this);
            ExchangeCommand = new ExchangeCommandImpl(this);
        }

        /// <summary>
        /// Google テストコマンド
        /// </summary>
        class GoogleCommandImpl : ICommand
        {
            private MainViewModel _vm;
            public GoogleCommandImpl(MainViewModel vm)
            {
                _vm = vm;
            }
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged = null;

            public async void Execute(object parameter)
            {
                await ExecuteAsync(parameter);
            }

            public async Task ExecuteAsync(object parameter)
            {
                // Google POP3サーバ設定
                _vm._Pop3.Pop3Config = new Pop3Config()
                {
                    Host = "pop.gmail.com",
                    Port = 995
                };
                _vm._Pop3.OAuthService = _vm._GoogleOAuth;

                // OAuth2認証
                _vm.LogMessage += $"Google OAuth2認証開始...\n";

                try
                {
                    await _vm._Pop3.AuthorizeAsync();
                    _vm.LogMessage += "トークン取得成功\n";
                }
                catch (Exception ex)
                {
                    _vm.LogMessage += $"失敗 ({ex.Message})\n";
                    return;
                }

                // メール一覧取得
                _vm.LogMessage += "メール一覧取得(MAX10件)...\n";
                try
                {
                    var subjects = await _vm._Pop3.GetMailSubjectsAsync();

                    if(subjects.Count() == 0)
                    {
                        _vm.LogMessage = "メールなし\n";
                    } else
                    {
                        _vm.LogMessage += string.Join("\n", subjects);
                    }
                }
                catch (Exception ex)
                {
                    _vm.LogMessage += $"失敗 ({ex.Message})\n";
                }

            }
        }

        /// <summary>
        /// Exchangeテストコマンド
        /// </summary>
        class ExchangeCommandImpl : ICommand
        {
            private MainViewModel _vm;
            public ExchangeCommandImpl(MainViewModel vm)
            {
                _vm = vm;
            }
            public bool CanExecute(object parameter) { return true; }
            public event EventHandler CanExecuteChanged = null;

            public async void Execute(object parameter)
            {
                await ExecuteAsync(parameter);
            }

            public async Task ExecuteAsync(object parameter)
            {
                // Exchange POP3 サーバ設定
                _vm._Pop3.Pop3Config = new Pop3Config()
                {
                    Host = "outlook.office365.com",
                    Port = 995
                };
                _vm._Pop3.OAuthService = _vm._ExchangeOAuth;

                // OAuth2認証
                _vm.LogMessage += $"Exchange OAuth2認証開始...\n";

                try
                {
                    await _vm._Pop3.AuthorizeAsync();
                    _vm.LogMessage += "トークン取得成功\n";
                }
                catch (Exception ex)
                {
                    _vm.LogMessage += $"失敗 ({ex.Message})\n";
                    return;
                }

                // メール一覧取得
                _vm.LogMessage += "メール一覧取得(MAX10件)...\n";
                try
                {
                    var subjects = await _vm._Pop3.GetMailSubjectsAsync();

                    if (subjects.Count() == 0)
                    {
                        _vm.LogMessage = "メールなし\n";
                    }
                    else
                    {
                        _vm.LogMessage += string.Join("\n", subjects);
                    }
                }
                catch (Exception ex)
                {
                    _vm.LogMessage += $"失敗 ({ex.Message})\n";
                }

            }
        }
    }
}
