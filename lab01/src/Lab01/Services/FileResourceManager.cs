using System.Text;

namespace Lab01;
public class FileResourceManager : IDisposable
{
    private readonly object _syncRoot = new();

    private FileStream? _fileStream;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    private bool _disposed;
    private readonly string _filePath;
    private readonly FileMode _fileMode;

    public FileResourceManager(string filePath, FileMode fileMode)
    {
        _filePath = filePath;
        _fileMode = fileMode;
    }

    public void OpenForWriting()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            try
            {
                _fileStream = new FileStream(_filePath, _fileMode, FileAccess.Write, FileShare.Read);
                _writer = new StreamWriter(_fileStream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Не удалось открыть файл '{_filePath}' для записи", ex);
                throw;
            }
        }
    }

    public void OpenForReading()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    throw new FileNotFoundException("Файл не найден", _filePath);
                }

                _fileStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _reader = new StreamReader(_fileStream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Не удалось открыть файл '{_filePath}' для чтения", ex);
                throw;
            }
        }
    }

    public void WriteLine(string text)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            if (_writer == null)
            {
                throw new InvalidOperationException("Файл не открыт для записи. Вызовите OpenForWriting().");
            }

            try
            {
                _writer.WriteLine(text);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при записи в файл '{_filePath}'", ex);
                throw;
            }
        }
    }

    public string ReadAllText()
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            if (_reader == null)
            {
                throw new InvalidOperationException("Файл не открыт для чтения. Вызовите OpenForReading().");
            }

            try
            {
                return _reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при чтении файла '{_filePath}'", ex);
                throw;
            }
        }
    }

    public void AppendText(string text)
    {
        ThrowIfDisposed();

        lock (_syncRoot)
        {
            try
            {
                using var fs = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(fs, Encoding.UTF8);
                writer.Write(text);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при добавлении текста в файл '{_filePath}'", ex);
                throw;
            }
        }
    }

    public string GetFileInfo()
    {
        ThrowIfDisposed();

        try
        {
            var info = new FileInfo(_filePath);

            if (!info.Exists)
            {
                return $"Файл '{_filePath}' не существует";
            }

            return $"Файл: {info.FullName}, размер: {info.Length} байт, создан: {info.CreationTime}";
        }
        catch (Exception ex)
        {
            Logger.LogError($"Ошибка при получении информации о файле '{_filePath}'", ex);
            throw;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(FileResourceManager));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {

            try
            {
                _writer?.Dispose();
                _reader?.Dispose();
                _fileStream?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при освобождении ресурсов файла '{_filePath}'", ex);
            }
        }

        _disposed = true;
    }

    ~FileResourceManager()
    {
    
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}


