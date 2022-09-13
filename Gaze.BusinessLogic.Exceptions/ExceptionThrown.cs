﻿using EventLibrary.Dialog;
using System;
namespace Gaze.BusinessLogic.Exceptions
{

    public class ExceptionThrown
    {
        #region Declarations
        frmExceptionDialog frmExceptionDialog = new frmExceptionDialog();
        #endregion

        #region methods
        /// <summary>
        /// Used to throw System Generated Exceptions
        /// </summary>
        /// <param name="ex">Stack Messages</param>
        /// <param name="Text">The Custom text to be displayed</param>
        public void ThrowNewStackException(Exception ex, string Text)
        {
            frmExceptionDialog frmExceptionDialog = new frmExceptionDialog
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Text = Text,
                TopMost = true
            };
            frmExceptionDialog.Show();
            
        }
        /// <summary>
        /// Used to throw custom exceptions, typically for custom error messages
        /// </summary>
        /// <param name="message">Message to show </param>
        /// <param name="stacktrace">Stack trace to show</param>
        /// <param name="Text">Title of the error message</param>
        public void ThrowNewException(string message,string stacktrace, string Text)
        {
            frmExceptionDialog frmExceptionDialog = new frmExceptionDialog
            {
                Message = message,
                StackTrace = stacktrace,
                Text = Text,
                TopMost = true
            };
            frmExceptionDialog.Show();

        }
        #endregion
    }
}
