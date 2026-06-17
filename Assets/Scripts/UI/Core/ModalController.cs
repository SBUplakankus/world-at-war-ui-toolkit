using UI.Constants;
using UI.Interfaces;
using UI.Records;
using UI.Views;
using UnityEngine.UIElements;

namespace UI.Core
{
    public class ModalController
    {
        private readonly ModalLayoutElements _elements;

        public ModalController(ModalLayoutElements elements)
        {
            _elements = elements;
        }

        public void Show()
        {
            _elements.ModalContainer.visible = true;
            _elements.ModalContainer.pickingMode = PickingMode.Position;
        }

        public void Hide()
        {
            _elements.ModalContainer.visible = false;
            _elements.ModalContainer.pickingMode = PickingMode.Ignore;
        }

        public void SetTitle(BaseView view)
        {
            _elements.Header.text = view is IScreen screen
                ? screen.HeaderName
                : ScreenNames.MainMenu;
        }
    }
}
