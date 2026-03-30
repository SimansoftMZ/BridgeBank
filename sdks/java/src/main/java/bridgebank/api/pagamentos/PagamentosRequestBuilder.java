package bridgebank.api.pagamentos;

import bridgebank.api.pagamentos.gerar.GerarRequestBuilder;
import com.microsoft.kiota.BaseRequestBuilder;
import com.microsoft.kiota.RequestAdapter;
import java.util.HashMap;
import java.util.Objects;
/**
 * Builds and executes requests for operations under /api/pagamentos
 */
@jakarta.annotation.Generated("com.microsoft.kiota")
public class PagamentosRequestBuilder extends BaseRequestBuilder {
    /**
     * The gerar property
     * @return a {@link GerarRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public GerarRequestBuilder gerar() {
        return new GerarRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * Instantiates a new {@link PagamentosRequestBuilder} and sets the default values.
     * @param pathParameters Path parameters for the request
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public PagamentosRequestBuilder(@jakarta.annotation.Nonnull final HashMap<String, Object> pathParameters, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/pagamentos", pathParameters);
    }
    /**
     * Instantiates a new {@link PagamentosRequestBuilder} and sets the default values.
     * @param rawUrl The raw URL to use for the request builder.
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public PagamentosRequestBuilder(@jakarta.annotation.Nonnull final String rawUrl, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/pagamentos", rawUrl);
    }
}
