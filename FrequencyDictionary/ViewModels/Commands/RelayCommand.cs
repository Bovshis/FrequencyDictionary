using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyDictionary.ViewModels.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<object> execute) : this(execute, canExecute: null)
        {
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            this._execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter)
        {
            return this._canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
