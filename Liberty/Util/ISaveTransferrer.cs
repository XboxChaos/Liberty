using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberty.Util
{
    /// <summary>
    /// Represents an object which transfers save data to an external storage device.
    /// </summary>
    public interface ISaveTransferrer
    {
        /// <summary>
        /// Starts transferring data in a separate thread.
        /// </summary>
        void Start();

        /// <summary>
        /// Fired when the ISaveTransferrer starts transferring a file.
        /// </summary>
        event EventHandler<TransferFileEventArgs> NextFile;

        /// <summary>
        /// Fired after each chunk of bytes has been transferred, updating information about the transfer progress.
        /// </summary>
        event EventHandler<TransferProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Fired when all files have been transferred.
        /// </summary>
        event EventHandler Done;
    }

    /// <summary>
    /// EventArgs for the ISaveTransferrer.NextFile event
    /// </summary>
    public class TransferFileEventArgs : EventArgs
    {
        public TransferFileEventArgs(string deviceName, string fileName, int sizeInBytes)
        {
            _deviceName = deviceName;
            _fileName = fileName;
            _sizeInBytes = sizeInBytes;
        }

        /// <summary>
        /// The name of the device which the file will be transferred to.
        /// </summary>
        public string DeviceName { get { return _deviceName; } }

        /// <summary>
        /// The name of the file which will be transferred.
        /// </summary>
        public string FileName { get { return _fileName; } }

        /// <summary>
        /// The file's size in bytes.
        /// </summary>
        public int SizeInBytes { get { return _sizeInBytes; } }

        private string _deviceName;
        private string _fileName;
        private int _sizeInBytes;
    }

    /// <summary>
    /// EventArgs for the ISaveTransferrer.ProgressChanged event
    /// </summary>
    public class TransferProgressEventArgs : EventArgs
    {
        public TransferProgressEventArgs(int bytesTransferred, int bytesTotal)
        {
            _bytesTransferred = bytesTransferred;
            _bytesTotal = bytesTotal;
        }

        /// <summary>
        /// The amount of bytes that have been transferred.
        /// </summary>
        public int BytesTransferred { get { return _bytesTransferred; } }

        /// <summary>
        /// The total amount of bytes in the file.
        /// This will always be the same as the SizeInBytes member of the file's NextFile event.
        /// </summary>
        public int BytesTotal { get { return _bytesTotal; } }

        private int _bytesTransferred;
        private int _bytesTotal;
    }

    /// <summary>
    /// EventArgs for the ISaveTransferrer.Done event
    /// </summary>
    public class TransferDoneEventArgs : EventArgs
    {
        public TransferDoneEventArgs(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        /// If transfer failed, the Exception that triggered this event.
        /// Will be null if the transfer succeeded.
        /// </summary>
        public Exception Error { get { return _exception; } }

        private Exception _exception;
    }
}
