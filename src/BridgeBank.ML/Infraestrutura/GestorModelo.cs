using Microsoft.ML;

namespace Simansoft.BridgeBank.ML.Infraestrutura;

/// <summary>
/// Gere o carregamento e caching de modelos ML.NET.
/// Thread-safe com lazy initialization.
/// </summary>
public class GestorModelo
{
    private readonly MLContext _mlContext = new();
    private readonly Dictionary<string, ITransformer> _modelos = [];
    private readonly Lock _lock = new();

    /// <summary>
    /// Carrega um modelo do disco (ou devolve do cache se já carregado)
    /// </summary>
    public ITransformer CarregarModelo(string caminhoModelo)
    {
        lock (_lock)
        {
            if (_modelos.TryGetValue(caminhoModelo, out var modelo))
                return modelo;

            if (!File.Exists(caminhoModelo))
                throw new FileNotFoundException($"Modelo ML não encontrado: {caminhoModelo}", caminhoModelo);

            modelo = _mlContext.Model.Load(caminhoModelo, out _);
            _modelos[caminhoModelo] = modelo;
            return modelo;
        }
    }

    /// <summary>
    /// Verifica se um modelo existe no disco
    /// </summary>
    public static bool ModeloExiste(string caminhoModelo) => File.Exists(caminhoModelo);

    /// <summary>
    /// Remove um modelo do cache (para re-carregamento após re-treino)
    /// </summary>
    public void InvalidarCache(string caminhoModelo)
    {
        lock (_lock)
        {
            _modelos.Remove(caminhoModelo);
        }
    }
}
