package bridgebank.models;

import com.microsoft.kiota.serialization.AdditionalDataHolder;
import com.microsoft.kiota.serialization.Parsable;
import com.microsoft.kiota.serialization.ParseNode;
import com.microsoft.kiota.serialization.SerializationWriter;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;
@jakarta.annotation.Generated("com.microsoft.kiota")
public class ReconciliarRequest implements AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    private Map<String, Object> additionalData;
    /**
     * The lancamentosERP property
     */
    private java.util.List<LancamentoERPDto> lancamentosERP;
    /**
     * The transacoes property
     */
    private java.util.List<TransacaoDto> transacoes;
    /**
     * Instantiates a new {@link ReconciliarRequest} and sets the default values.
     */
    public ReconciliarRequest() {
        this.setAdditionalData(new HashMap<>());
    }
    /**
     * Creates a new instance of the appropriate class based on discriminator value
     * @param parseNode The parse node to use to read the discriminator value and create the object
     * @return a {@link ReconciliarRequest}
     */
    @jakarta.annotation.Nonnull
    public static ReconciliarRequest createFromDiscriminatorValue(@jakarta.annotation.Nonnull final ParseNode parseNode) {
        Objects.requireNonNull(parseNode);
        return new ReconciliarRequest();
    }
    /**
     * Gets the AdditionalData property value. Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     * @return a {@link Map<String, Object>}
     */
    @jakarta.annotation.Nonnull
    public Map<String, Object> getAdditionalData() {
        return this.additionalData;
    }
    /**
     * The deserialization information for the current model
     * @return a {@link Map<String, java.util.function.Consumer<ParseNode>>}
     */
    @jakarta.annotation.Nonnull
    public Map<String, java.util.function.Consumer<ParseNode>> getFieldDeserializers() {
        final HashMap<String, java.util.function.Consumer<ParseNode>> deserializerMap = new HashMap<String, java.util.function.Consumer<ParseNode>>(2);
        deserializerMap.put("lancamentosERP", (n) -> { this.setLancamentosERP(n.getCollectionOfObjectValues(LancamentoERPDto::createFromDiscriminatorValue)); });
        deserializerMap.put("transacoes", (n) -> { this.setTransacoes(n.getCollectionOfObjectValues(TransacaoDto::createFromDiscriminatorValue)); });
        return deserializerMap;
    }
    /**
     * Gets the lancamentosERP property value. The lancamentosERP property
     * @return a {@link java.util.List<LancamentoERPDto>}
     */
    @jakarta.annotation.Nullable
    public java.util.List<LancamentoERPDto> getLancamentosERP() {
        return this.lancamentosERP;
    }
    /**
     * Gets the transacoes property value. The transacoes property
     * @return a {@link java.util.List<TransacaoDto>}
     */
    @jakarta.annotation.Nullable
    public java.util.List<TransacaoDto> getTransacoes() {
        return this.transacoes;
    }
    /**
     * Serializes information the current object
     * @param writer Serialization writer to use to serialize this model
     */
    public void serialize(@jakarta.annotation.Nonnull final SerializationWriter writer) {
        Objects.requireNonNull(writer);
        writer.writeCollectionOfObjectValues("lancamentosERP", this.getLancamentosERP());
        writer.writeCollectionOfObjectValues("transacoes", this.getTransacoes());
        writer.writeAdditionalData(this.getAdditionalData());
    }
    /**
     * Sets the AdditionalData property value. Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     * @param value Value to set for the AdditionalData property.
     */
    public void setAdditionalData(@jakarta.annotation.Nullable final Map<String, Object> value) {
        this.additionalData = value;
    }
    /**
     * Sets the lancamentosERP property value. The lancamentosERP property
     * @param value Value to set for the lancamentosERP property.
     */
    public void setLancamentosERP(@jakarta.annotation.Nullable final java.util.List<LancamentoERPDto> value) {
        this.lancamentosERP = value;
    }
    /**
     * Sets the transacoes property value. The transacoes property
     * @param value Value to set for the transacoes property.
     */
    public void setTransacoes(@jakarta.annotation.Nullable final java.util.List<TransacaoDto> value) {
        this.transacoes = value;
    }
}
