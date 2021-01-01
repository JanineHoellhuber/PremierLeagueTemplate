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
        private ObservableCollection<Team> _homeTeam;
        private ObservableCollection<Team> _guestTeam;
        private Team _selectedHomeTeam;
        private Team _selectedGuestTeam;
        private Game _game;
        private int _round;
        private int _homeGoals;
        private int _guestGoals;

        public ObservableCollection<Team> HomeTeam
        {
            get => _homeTeam;
            set
            {
                _homeTeam = value;
                OnPropertyChanged(nameof(HomeTeam));
                Validate();
            }
        }

        public ObservableCollection<Team> GuestTeam
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

        public int Guestgoals
        {
            get => _guestGoals;
            set
            {
                _guestGoals = value;
                OnPropertyChanged(nameof(Guestgoals));
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

        public Team SelectedHomeTeam
        {
            get => _selectedHomeTeam;
            set
            {
                _selectedHomeTeam = value;
                OnPropertyChanged(nameof(SelectedHomeTeam));
                Validate();
            }
        }

        public Team SelectedGuestTeam
        {
            get => _selectedGuestTeam;
            set
            {
                _selectedGuestTeam = value;
                OnPropertyChanged(nameof(SelectedGuestTeam));
                Validate();
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
            if (Round < 1 && Round > 38)
            {
                yield return new ValidationResult("$Round has to be between 1 and 38", new string[] { nameof(Round) });
            }
            if (SelectedGuestTeam == SelectedHomeTeam)
            { 
                yield return new ValidationResult("$Hometeam is same as Guestteam", new string[] { nameof(HomeTeam) });

            }
            if (Homegoals < 0)
            {
                yield return new ValidationResult("$Homegoals are < 0", new string[] { nameof(Homegoals) });

            }
            if (Guestgoals < 0)
            {
                yield return new ValidationResult("$Guestgoals are < 0", new string[] { nameof(Guestgoals) });

            }
        }

        public async Task LoadHomeTeams()
        {
            using IUnitOfWork uow = new UnitOfWork();
            var hometeams = await uow.Teams.GetAllTeamsAsync();
            HomeTeam = new ObservableCollection<Team>(hometeams);
            _selectedHomeTeam = HomeTeam.First();


        }
        public async Task LoadGuestTeams()
        {
            using IUnitOfWork uow = new UnitOfWork();
            var guestteams = await uow.Teams.GetAllTeamsAsync();
            GuestTeam = new ObservableCollection<Team>(guestteams);
            _selectedGuestTeam = GuestTeam.First();


        }

        private ICommand _cmdSave;
        public ICommand CmdSave
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
                                var HomeTeam = await uow.Teams.GetById(SelectedHomeTeam.Id);
                                var GuestTeam = await uow.Teams.GetById(SelectedGuestTeam.Id);
                                Games = new Game
                                {
                                    Round = Round,
                                    HomeTeam = HomeTeam,
                                    GuestTeam = GuestTeam,
                                    HomeGoals = Homegoals,
                                    GuestGoals = Guestgoals
                                };
                                await uow.Games.AddAsync(Games);
                                await uow.SaveChangesAsync();
                                Controller.CloseWindow(this);
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
                    canExecute: _ => IsValid
                    );
                }
                return _cmdSave;
            }
            set => _cmdSave = value;
        }
    }
}

