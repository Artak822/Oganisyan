using System.IO;

namespace Lab1
{
    public class FileResourceManager : IDisposable
    {
        private FileStream? _fileStream;
        private StreamWriter? _writer;
        private StreamReader? _reader;
        private bool _disposed = false;
        private readonly string _filePath;

        public FileResourceManager(string filePath, FileMode fileMode = FileMode.OpenOrCreate)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            
            try
            {
                _fileStream = new FileStream(_filePath, fileMode, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось открыть файл: {_filePath}", ex);
            }
        }

        public void OpenForWriting()
        {
            ThrowIfDisposed();
            
            if (_writer != null)
            {
                return;
            }

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            try
            {
                _fileStream?.Seek(0, SeekOrigin.End);
                _writer = new StreamWriter(_fileStream!, leaveOpen: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось открыть файл для записи: {_filePath}", ex);
            }
        }

        public void OpenForReading()
        {
            ThrowIfDisposed();
            
            if (_reader != null)
            {
                return;
            }

            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            try
            {
                _fileStream?.Seek(0, SeekOrigin.Begin);
                _reader = new StreamReader(_fileStream!, leaveOpen: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось открыть файл для чтения: {_filePath}", ex);
            }
        }

        public void WriteLine(string text)
        {
            ThrowIfDisposed();
            
            if (_writer == null)
            {
                OpenForWriting();
            }

            try
            {
                _writer!.WriteLine(text);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при записи в файл: {_filePath}", ex);
            }
        }

        public string ReadAllText()
        {
            ThrowIfDisposed();
            
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {_filePath}");
            }

            if (_reader == null)
            {
                OpenForReading();
            }

            try
            {
                _fileStream!.Seek(0, SeekOrigin.Begin);
                return _reader!.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при чтении файла: {_filePath}", ex);
            }
        }

        public void AppendText(string text)
        {
            ThrowIfDisposed();
            
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {_filePath}");
            }

            if (_writer == null)
            {
                OpenForWriting();
            }

            try
            {
                _fileStream!.Seek(0, SeekOrigin.End);
                _writer!.Write(text);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при добавлении текста в файл: {_filePath}", ex);
            }
        }

        public FileInfo GetFileInfo()
        {
            ThrowIfDisposed();
            
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл не найден: {_filePath}");
            }

            try
            {
                return new FileInfo(_filePath);
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при получении информации о файле: {_filePath}", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    using (_writer)
                    {
                        _writer?.Dispose();
                    }
                    _writer = null;

                    using (_reader)
                    {
                        _reader?.Dispose();
                    }
                    _reader = null;

                    using (_fileStream)
                    {
                        _fileStream?.Dispose();
                    }
                    _fileStream = null;
                }

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FileResourceManager), "Объект уже освобожден");
            }
        }

        ~FileResourceManager()
        {
            Dispose(false);
        }
    }
}

