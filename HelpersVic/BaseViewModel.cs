using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MelodiaTherapy;

public class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;
	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyname = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs((propertyname)));
	}
	protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyname = null)
	{
		if (EqualityComparer<T>.Default.Equals(storage, value))
		{
			return false;
		}
		storage = value;
		OnPropertyChanged(propertyname);
		return true;
	}

	public BaseViewModel()
	{

	}
}

public interface IMenuDrawerVM
{
    public void OnDrawerCloseClicked();
	public void ShowDrawer();
	public void HideDrawer();
}