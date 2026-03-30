package bridgebank.api.bancos;

import bridgebank.api.bancos.geradores.GeradoresRequestBuilder;
import bridgebank.api.bancos.parsers.ParsersRequestBuilder;
import com.microsoft.kiota.BaseRequestBuilder;
import com.microsoft.kiota.RequestAdapter;
import java.util.HashMap;
import java.util.Objects;
/**
 * Builds and executes requests for operations under /api/bancos
 */
@jakarta.annotation.Generated("com.microsoft.kiota")
public class BancosRequestBuilder extends BaseRequestBuilder {
    /**
     * The geradores property
     * @return a {@link GeradoresRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public GeradoresRequestBuilder geradores() {
        return new GeradoresRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * The parsers property
     * @return a {@link ParsersRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public ParsersRequestBuilder parsers() {
        return new ParsersRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * Instantiates a new {@link BancosRequestBuilder} and sets the default values.
     * @param pathParameters Path parameters for the request
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public BancosRequestBuilder(@jakarta.annotation.Nonnull final HashMap<String, Object> pathParameters, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/bancos", pathParameters);
    }
    /**
     * Instantiates a new {@link BancosRequestBuilder} and sets the default values.
     * @param rawUrl The raw URL to use for the request builder.
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public BancosRequestBuilder(@jakarta.annotation.Nonnull final String rawUrl, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/bancos", rawUrl);
    }
}
