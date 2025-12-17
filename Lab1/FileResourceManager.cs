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

        // Конструктор, принимающий путь к файлу и режим открытия
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

        // Открывает файл для записи
        public void OpenForWriting()
        {
            ThrowIfDisposed();
            
            if (_writer != null)
            {
                return; // Уже открыт для записи
            }

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            try
            {
                _fileStream?.Seek(0, SeekOrigin.End); // Перемещаемся в конец файла
                _writer = new StreamWriter(_fileStream!, leaveOpen: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось открыть файл для записи: {_filePath}", ex);
            }
        }

        // Открывает файл для чтения
        public void OpenForReading()
        {
            ThrowIfDisposed();
            
            if (_reader != null)
            {
                return; // Уже открыт для чтения
            }

            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            try
            {
                _fileStream?.Seek(0, SeekOrigin.Begin); // Перемещаемся в начало файла
                _reader = new StreamReader(_fileStream!, leaveOpen: true);
            }
            catch (Exception ex)
            {
                throw new IOException($"Не удалось открыть файл для чтения: {_filePath}", ex);
            }
        }

        // Записывает строку в файл
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
                _writer.Flush(); // Принудительно записываем в файл
            }
            catch (Exception ex)
            {
                throw new IOException($"Ошибка при записи в файл: {_filePath}", ex);
            }
        }

        // Читает весь файл
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

        // Добавляет текст в конец файла
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

        // Возвращает информацию о файле (размер, дата создания)
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

        // Реализация IDisposable
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
                    // Освобождаем управляемые ресурсы
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

        // Проверка на освобождение ресурсов
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(FileResourceManager), "Объект уже освобожден");
            }
        }

        // Финализатор (на случай, если Dispose не был вызван)
        ~FileResourceManager()
        {
            Dispose(false);
        }
    }
}

