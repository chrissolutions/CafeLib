using System;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Mobile.Test.Core.Fakes;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using FileMode = System.IO.FileMode;
using FileAccess = System.IO.FileAccess;
using FileShare = System.IO.FileShare;
using Stream = System.IO.Stream;
// ReSharper disable UnusedMember.Global

[assembly: Dependency(typeof(FakeDeserializer))]
[assembly: Dependency(typeof(FakeResourcesProvider))]

namespace CafeLib.Mobile.Test.Core.Fakes
{
    internal class FakePlatformServices : IPlatformServices
    {
        private readonly Action<Action> _invokeOnMainThread;
        private readonly Action<Uri> _openUriAction;
        private readonly Func<Uri, CancellationToken, Task<Stream>> _getStreamAsync;
        private readonly Func<VisualElement, double, double, SizeRequest> _getNativeSizeFunc;
        private readonly bool _useRealisticLabelMeasure;

        public FakePlatformServices(Action<Action> invokeOnMainThread = null, Action<Uri> openUriAction = null,
            Func<Uri, CancellationToken, Task<Stream>> getStreamAsync = null,
            Func<VisualElement, double, double, SizeRequest> getNativeSizeFunc = null,
            bool useRealisticLabelMeasure = false, bool isInvokeRequired = false)
        {
            _invokeOnMainThread = invokeOnMainThread;
            _openUriAction = openUriAction;
            _getStreamAsync = getStreamAsync;
            _getNativeSizeFunc = getNativeSizeFunc;
            _useRealisticLabelMeasure = useRealisticLabelMeasure;
            IsInvokeRequired = isInvokeRequired;
        }

        private static readonly MD5CryptoServiceProvider Checksum = new MD5CryptoServiceProvider();

        public string GetMD5Hash(string input)
        {
            var bytes = Checksum.ComputeHash(Encoding.UTF8.GetBytes(input));
            var ret = new char[32];
            for (int i = 0; i < 16; i++)
            {
                ret[i * 2] = (char)Hex(bytes[i] >> 4);
                ret[i * 2 + 1] = (char)Hex(bytes[i] & 0xf);
            }
            return new string(ret);
        }

        private static int Hex(int v)
        {
            if (v < 10)
                return '0' + v;
            return 'a' + v - 10;
        }

        public double GetNamedSize(NamedSize size, Type targetElement, bool useOldSizes)
        {
            switch (size)
            {
                case NamedSize.Default:
                    return 10;
                case NamedSize.Micro:
                    return 4;
                case NamedSize.Small:
                    return 8;
                case NamedSize.Medium:
                    return 12;
                case NamedSize.Large:
                    return 16;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size));
            }
        }

        public Color GetNamedColor(string name)
        {
            throw new NotImplementedException();
        }

        public void OpenUriAction(Uri uri)
        {
            if (_openUriAction != null)
                _openUriAction(uri);
            else
                throw new NotImplementedException();
        }

        public bool IsInvokeRequired { get; }
        public OSAppTheme RequestedTheme { get; }

        public string RuntimePlatform { get; set; }

        public void BeginInvokeOnMainThread(Action action)
        {
            if (_invokeOnMainThread == null)
                action();
            else
                _invokeOnMainThread(action);
        }

        public Ticker CreateTicker()
        {
            return new FakeTicker();
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            Timer timer = null;

            void OnTimeout(object o) =>
                BeginInvokeOnMainThread(() =>
                {
                    if (callback()) return;
                    // ReSharper disable once AccessToModifiedClosure
                    timer?.Dispose();
                });

            timer = new Timer(OnTimeout, null, interval, interval);
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (_getStreamAsync == null)
                throw new NotImplementedException();
            return _getStreamAsync(uri, cancellationToken);
        }

        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            return new MockIsolatedStorageFile(IsolatedStorageFile.GetUserStoreForAssembly());
        }

        public class MockIsolatedStorageFile : IIsolatedStorageFile
        {
            private readonly IsolatedStorageFile _isolatedStorageFile;

            public MockIsolatedStorageFile(IsolatedStorageFile isolatedStorageFile)
            {
                _isolatedStorageFile = isolatedStorageFile;
            }

            public Task<bool> GetDirectoryExistsAsync(string path)
            {
                return Task.FromResult(_isolatedStorageFile.DirectoryExists(path));
            }

            public Task CreateDirectoryAsync(string path)
            {
                _isolatedStorageFile.CreateDirectory(path);
                return Task.FromResult(true);
            }

            public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access)
            {
                Stream stream = _isolatedStorageFile.OpenFile(path, mode, access);
                return Task.FromResult(stream);
            }

            public Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share)
            {
                Stream stream = _isolatedStorageFile.OpenFile(path, mode, access, share);
                return Task.FromResult(stream);
            }

            public Task<bool> GetFileExistsAsync(string path)
            {
                return Task.FromResult(_isolatedStorageFile.FileExists(path));
            }

            public Task<DateTimeOffset> GetLastWriteTimeAsync(string path)
            {
                return Task.FromResult(_isolatedStorageFile.GetLastWriteTime(path));
            }
        }

        public void QuitApplication()
        {

        }

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            if (_getNativeSizeFunc != null)
                return _getNativeSizeFunc(view, widthConstraint, heightConstraint);
            // EVERYTHING IS 100 x 20

            if (view is Label label && _useRealisticLabelMeasure)
            {
                var letterSize = new Size(5, 10);
                var w = label.Text.Length * letterSize.Width;
                var h = letterSize.Height;
                if (!double.IsPositiveInfinity(widthConstraint) && w > widthConstraint)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    h = ((int)w / (int)widthConstraint) * letterSize.Height;
                    w = widthConstraint - (widthConstraint % letterSize.Width);

                }
                return new SizeRequest(new Size(w, h), new Size(Math.Min(10, w), h));
            }

            return new SizeRequest(new Size(100, 20));
        }
    }
}