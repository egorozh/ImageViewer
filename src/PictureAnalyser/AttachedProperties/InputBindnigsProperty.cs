using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace PictureAnalyser
{
    /// <summary>
    /// Привязка коллекции <see cref="KeyBinding"/> к какому-либо <see cref="UIElement"/>
    /// </summary>
    public class InputBindnigsProperty : BaseAttachedProperty<InputBindnigsProperty, ObservableCollection<KeyBinding>>
    {
        private UIElement _uiElement;

        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UIElement uiElement)) return;

            _uiElement = uiElement;

            if (!(e.NewValue is ObservableCollection<KeyBinding> keyBindings)) return;

            uiElement.InputBindings.AddRange(keyBindings);

            keyBindings.CollectionChanged += KeyBindingsCollectionChanged;
        }

        private void KeyBindingsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                _uiElement.InputBindings.Add((KeyBinding) e.NewItems[0]);

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                _uiElement.InputBindings.Remove((KeyBinding) e.OldItems[0]);

                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _uiElement.InputBindings.Clear();

                return;
            }
        }
    }
}