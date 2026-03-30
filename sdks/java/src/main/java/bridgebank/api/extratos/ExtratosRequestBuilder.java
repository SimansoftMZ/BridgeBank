package bridgebank.api.extratos;

import bridgebank.api.extratos.parse.ParseRequestBuilder;
import com.microsoft.kiota.BaseRequestBuilder;
import com.microsoft.kiota.RequestAdapter;
import java.util.HashMap;
import java.util.Objects;
/**
 * Builds and executes requests for operations under /api/extratos
 */
@jakarta.annotation.Generated("com.microsoft.kiota")
public class ExtratosRequestBuilder extends BaseRequestBuilder {
    /**
     * The parse property
     * @return a {@link ParseRequestBuilder}
     */
    @jakarta.annotation.Nonnull
    public ParseRequestBuilder parse() {
        return new ParseRequestBuilder(pathParameters, requestAdapter);
    }
    /**
     * Instantiates a new {@link ExtratosRequestBuilder} and sets the default values.
     * @param pathParameters Path parameters for the request
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public ExtratosRequestBuilder(@jakarta.annotation.Nonnull final HashMap<String, Object> pathParameters, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/extratos", pathParameters);
    }
    /**
     * Instantiates a new {@link ExtratosRequestBuilder} and sets the default values.
     * @param rawUrl The raw URL to use for the request builder.
     * @param requestAdapter The request adapter to use to execute the requests.
     */
    public ExtratosRequestBuilder(@jakarta.annotation.Nonnull final String rawUrl, @jakarta.annotation.Nonnull final RequestAdapter requestAdapter) {
        super(requestAdapter, "{+baseurl}/api/extratos", rawUrl);
    }
}
