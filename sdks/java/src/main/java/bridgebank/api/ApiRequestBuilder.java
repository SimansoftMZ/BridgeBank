package bridgebank.api;

import bridgebank.api.bancos.BancosRequestBuilder;
import bridgebank.api.classificacao.ClassificacaoRequestBuilder;
import bridgebank.api.enums.EnumsRequestBuilder;
import bridgebank.api.extratos.ExtratosRequestBuilder;
import bridgebank.api.pagamentos.PagamentosRequestBuilder;
import bridgebank.api.reconciliacao.ReconciliacaoRequestBuilder;
import com.microsoft.kiota.BaseRequestBuilder;
import com.microsoft.kiota.RequestAdapter;
import java.util.HashMap;
import java.util.Objects;
/**
 * Builds and executes requests for operations under /api
 */
@jakarta.annotation.Generated("com.microsoft.kiota")
public class ApiRequestBuilder extends BaseRequestBuilder {
    /**
     * The bancos property
     * @return a {@link BancosRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public BancosRequestBuilder bancos() {
        return new BancosRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The classificacao property
     * @return a {@link ClassificacaoRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public ClassificacaoRequestBuilder classificacao() {
        return new ClassificacaoRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The enums property
     * @return a {@link EnumsRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public EnumsRequestBuilder enums() {
        return new EnumsRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The extratos property
     * @return a {@link ExtratosRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public ExtratosRequestBuilder extratos() {
        return new ExtratosRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The pagamentos property
     * @return a {@link PagamentosRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public PagamentosRequestBuilder pagamentos() {
        return new PagamentosRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The reconciliacao property
     * @return a {@link ReconciliacaoRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public ReconciliacaoRequestBuilder reconciliacao() {
        return new ReconciliacaoRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * Instantiates a new {@link ApiRequestBuilder} and sets the default values.
     * @param pathParameters Path parameters for the request
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public ApiRequestBuilder(@jakarta.annotation.Nonnull final HashMap<String, Object> pathParameters, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api", pathParameters);
    }
    /**
     * Instantiates a new {@link ApiRequestBuilder} and sets the default values.
     * @param rawUrl The raw URL to use for the request builder.
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public ApiRequestBuilder(@jakarta.annotation.Nonnull final String rawUrl, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api", rawUrl);
    }
}
