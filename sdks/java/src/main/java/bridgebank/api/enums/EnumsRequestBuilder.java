package bridgebank.api.enums;

import bridgebank.api.enums.categorias.CategoriasRequestBuilder;
import bridgebank.api.enums.tiposcorrespondencia.TiposCorrespondenciaRequestBuilder;
import bridgebank.api.enums.tipostransacao.TiposTransacaoRequestBuilder;
import com.microsoft.kiota.BaseRequestBuilder;
import com.microsoft.kiota.RequestAdapter;
import java.util.HashMap;
import java.util.Objects;
/**
 * Builds and executes requests for operations under /api/enums
 */
@jakarta.annotation.Generated("com.microsoft.kiota")
public class EnumsRequestBuilder extends BaseRequestBuilder {
    /**
     * The categorias property
     * @return a {@link CategoriasRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public CategoriasRequestBuilder categorias() {
        return new CategoriasRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The tiposCorrespondencia property
     * @return a {@link TiposCorrespondenciaRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public TiposCorrespondenciaRequestBuilder tiposCorrespondencia() {
        return new TiposCorrespondenciaRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The tiposTransacao property
     * @return a {@link TiposTransacaoRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public TiposTransacaoRequestBuilder tiposTransacao() {
        return new TiposTransacaoRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * Instantiates a new {@link EnumsRequestBuilder} and sets the default values.
     * @param pathParameters Path parameters for the request
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public EnumsRequestBuilder(@jakarta.annotation.Nonnull final HashMap<String, Object> pathParameters, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/enums", pathParameters);
    }
    /**
     * Instantiates a new {@link EnumsRequestBuilder} and sets the default values.
     * @param rawUrl The raw URL to use for the request builder.
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public EnumsRequestBuilder(@jakarta.annotation.Nonnull final String rawUrl, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/enums", rawUrl);
    }
}
