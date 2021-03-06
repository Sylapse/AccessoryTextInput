// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using UIKit;
using ObjCRuntime;
using System.Linq;

namespace Sylapse.AccessoryTextInput
{
    public partial class TextInputObject : UITextField
    {
        private InputView _inputView;
        private Action<string> _callback;
        private string _hint;
        private NSLayoutConstraint _maxHeightConstraint;

        private nfloat _maxHeight = 128.0f;
        public nfloat MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; _maxHeightConstraint.Constant = value; }
        }

        public new UIColor BackgroundColor
        {
            get { return _inputView.BackgroundColor; }
            set { _inputView.BackgroundColor = value; }
        }

        public UIColor ButtonTextColor
        {
            get { return _inputView.DoneButton.TintColor; }
            set { _inputView.DoneButton.TintColor = value; }
        }

        public TextInputObject()
        {
            var arr = NSBundle.MainBundle.LoadNib("InputView", null, null);
            _inputView = Runtime.GetNSObject<InputView>(arr.ValueAt(0));
            _inputView.TranslatesAutoresizingMaskIntoConstraints = false;

            _inputView.TextView.Changed += TextView_Changed;
            _inputView.DoneButton.TouchUpInside += DoneButton_TouchUpInside;

            SetConstraints();
            InputAccessoryView = _inputView;

            BackgroundColor = AccessoryTextInputAppearance.BackgroundColor;
            ButtonTextColor = AccessoryTextInputAppearance.ButtonTextColor;
        }

        public void GetInput(string hint, string initialText, Action<string> callback, UIKeyboardType keyboardType = UIKeyboardType.Default, UITextAutocapitalizationType capitalizationType = UITextAutocapitalizationType.Sentences)
        {
            _inputView.TextView.Text = initialText;
            _inputView.TextView.KeyboardType = keyboardType;
            _inputView.TextView.AutocapitalizationType = capitalizationType;
            _callback = callback;
            _hint = hint;
            SetHint();
            DisableScrolling(); // Disable scrolling when the input is shown. It could still be on from the last usage
            BecomeFirstResponder();
        }

        public void Finish()
        {
            if (_inputView.TextView.IsFirstResponder)
            {
                _callback(_inputView.TextView.Text);
                _callback = null;
                ResignFirstResponder();
            }
        }

        public void Cancel()
        {
            _callback = null;
            ResignFirstResponder();
        }

        public override bool BecomeFirstResponder()
        {
            base.BecomeFirstResponder();
            _inputView.TextView.BecomeFirstResponder();

            // Remove the automatic height constraint on the accessory input view to allow the custom views to size themselves appropriately
            RemoveAutomaticHeightConstraint();
            return true;
        }

        public override bool ResignFirstResponder()
        {
            _inputView.TextView.ResignFirstResponder();
            base.ResignFirstResponder();
            return true;
        }

        private void DoneButton_TouchUpInside(object sender, EventArgs e)
        {
            _callback(_inputView.TextView.Text);
            _callback = null;
            ResignFirstResponder();
        }

        private void TextView_Changed(object sender, EventArgs e)
        {
            SetHint();

            if (_inputView.TextView.ContentSize.Height >= MaxHeight - 16)
            { // 16 is two lots of the padding top and bottom of the TextView
                _inputView.TextView.ScrollEnabled = true;
            }
            else if (_inputView.TextView.ScrollEnabled)
            {
                DisableScrolling();
            }

            // The height constraint can be added back by iOS due to rotation or backgrounding and foregrounding the app while editing, checking here solves it.
            RemoveAutomaticHeightConstraint();
        }

        private void SetHint()
        {
            if (string.IsNullOrEmpty(_inputView.TextView.Text))
            {
                _inputView.TextField.Placeholder = _hint;
            }
            else {
                _inputView.TextField.Placeholder = null;
            }
        }

        private void DisableScrolling()
        {
            _inputView.TextView.ScrollEnabled = false;
            _inputView.TextView.SizeToFit();
        }

        private void SetConstraints()
        {
            _maxHeightConstraint = NSLayoutConstraint.Create(_inputView,
                NSLayoutAttribute.Height,
                NSLayoutRelation.LessThanOrEqual,
                null,
                NSLayoutAttribute.NoAttribute,
                0, MaxHeight);


            var minHeightConstraint = NSLayoutConstraint.Create(_inputView,
                NSLayoutAttribute.Height,
                NSLayoutRelation.GreaterThanOrEqual,
                null,
                NSLayoutAttribute.NoAttribute,
                0, 44);

            _inputView.AddConstraint(_maxHeightConstraint);
            _inputView.AddConstraint(minHeightConstraint);
        }

        // iOS adds a height constraint to accessory input views. In this case it stops the TextView growing so remove it.
        private void RemoveAutomaticHeightConstraint()
        {
            var accessoryParentConstraints = _inputView.InputAccessoryView.Superview.Constraints;
            var heightConstraint = accessoryParentConstraints.FirstOrDefault(c =>
               c.FirstItem == _inputView &&
               c.FirstAttribute == NSLayoutAttribute.Height &&
               c.SecondItem == null &&
               c.SecondAttribute == NSLayoutAttribute.NoAttribute &&
               c.Relation == NSLayoutRelation.Equal);

            if (heightConstraint != null)
            {
                InputAccessoryView.Superview.RemoveConstraint(heightConstraint);
            }
        }
    }
}
