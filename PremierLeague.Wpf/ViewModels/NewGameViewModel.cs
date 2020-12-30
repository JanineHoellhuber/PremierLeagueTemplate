using PremierLeague.Core.Contracts;
using PremierLeague.Core.Entities;
using PremierLeague.Persistence;
using PremierLeague.Wpf.Common;
using PremierLeague.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PremierLeague.Wpf.ViewModels
{
    class NewGameViewModel : BaseViewModel
    {
        private IWindowController _controller;
        private ObservableCollection<Game> _homeTeam;
        private ObservableCollection<Game> _guestTeam;
        private Game _selectedHomeTeam;
        private Game _selectedGuestTeam;
        private Game _game;
        private int _round;
        private int _homeGoals;
        private int _guestGoals;

        public ObservableCollection<Game> HomeTeam
        {
            get => _homeTeam;
            set
            {
                _homeTeam = value;
                OnPropertyChanged(nameof(HomeTeam));
                Validate();
            }
        }

        public ObservableCollection<Game> GuestTeam
        {
            get => _guestTeam;
            set
            {
                _guestTeam = value;
                OnPropertyChanged(nameof(GuestTeam));
                Validate();
            }
        }

        public int Round
        {
            get => _round;
            set
            {
                _round = value;
                OnPropertyChanged(nameof(Round));
                Validate();
            }
        }
        public int Homegoals
        {
            get => _homeGoals;
            set
            {
                _homeGoals = value;
                OnPropertyChanged(nameof(Homegoals));
                Validate();

            }
        }

        public int GuestGoals
        {
            get => _guestGoals;
            set
            {
                _guestGoals = value;
                OnPropertyChanged(nameof(GuestGoals));
                Validate();
            }
        }

        public Game Games
        {
            get => _game;
            set
            {
                _game = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        public Game SelectedHomeTeam
        {
            get => _selectedHomeTeam;
            set
            {
                _selectedHomeTeam = value;
                OnPropertyChanged(nameof(SelectedHomeTeam));
            }
        }

        public Game SelectedGuestTeam
        {
            get => _selectedGuestTeam;
            set
            {
                _selectedGuestTeam = value;
                OnPropertyChanged(nameof(SelectedGuestTeam));
            }
        }

        public NewGameViewModel(IWindowController controller) : base(controller)
        {
            _controller = controller;
            LoadHomeTeams();
            LoadGuestTeams();

        }
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Round > 1 && Round < 38)
            {
                yield return new ValidationResult("$Round has to be between 1 and 38", new string[] { nameof(Round) });
            }
            if (HomeTeam.Equals(GuestTeam))
            {
                yield return new ValidationResult("$Hometeam is same as Guestteam", new string[] { nameof(HomeTeam) });

            }
            if (Homegoals < 0)
            {
                yield return new ValidationResult("$Homegoals are < 0", new string[] { nameof(Homegoals) });

            }
            if (GuestGoals < 0)
            {
                yield return new ValidationResult("$Guestgoals are < 0", new string[] { nameof(GuestGoals) });

            }
        }

        public async Task LoadHomeTeams()
        {
            using IUnitOfWork uow = new UnitOfWork();
            var hometeams = await uow.Games.GetAllGamesAsync();
            HomeTeam = new ObservableCollection<Game>(hometeams);
            _selectedHomeTeam = HomeTeam.First();


        }
        public async Task LoadGuestTeams()
        {
            using IUnitOfWork uow = new UnitOfWork();
            var guestteams = await uow.Games.GetAllGamesAsync();
            GuestTeam = new ObservableCollection<Game>(guestteams);
            _selectedGuestTeam = GuestTeam.First();


        }

        private ICommand _cmdSave;
        public ICommand _CmdSave
        {
            get
            {
                if (_cmdSave == null)
                {
                    _cmdSave = new RelayCommand(
                        execute: async _ =>
                        {
                            try
                            {
                                using IUnitOfWork uow = new UnitOfWork();
                                Games = new Game()
                                {
                                    Round = Round,
                                    HomeGoals = Homegoals,
                                    GuestGoals = GuestGoals
                                };
                                await uow.Games.AddAsync(Games);
                            }
                            catch (ValidationException ve)
                            {
                                if (ve.Value is IEnumerable<string> properties)
                                {
                                    foreach (var property in properties)
                                    {
                                        Errors.Add(property, new List<string> { ve.ValidationResult.ErrorMessage });
                                    }
                                }
                                else
                                {
                                    DbError = ve.ValidationResult.ToString();
                                }
                            }
                        },
                    canExecute: _ => !HasErrors
                    );
                }
                return _cmdSave;
            }
            set => _cmdSave = value;
        }
    }
}

