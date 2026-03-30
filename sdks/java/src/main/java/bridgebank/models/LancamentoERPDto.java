package bridgebank.models;

import com.microsoft.kiota.serialization.AdditionalDataHolder;
import com.microsoft.kiota.serialization.Parsable;
import com.microsoft.kiota.serialization.ParseNode;
import com.microsoft.kiota.serialization.SerializationWriter;
import com.microsoft.kiota.serialization.UntypedNode;
import java.time.OffsetDateTime;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;
@jakarta.annotation.Generated("com.microsoft.kiota")
public class LancamentoERPDto implements AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    private Map<String, Object> additionalData;
    /**
     * The cliente property
     */
    private String cliente;
    /**
     * The data property
     */
    private OffsetDateTime data;
    /**
     * The descricao property
     */
    private String descricao;
    /**
     * The fornecedor property
     */
    private String fornecedor;
    /**
     * The id property
     */
    private String id;
    /**
     * The numeroDocumento property
     */
    private String numeroDocumento;
    /**
     * The referencia property
     */
    private String referencia;
    /**
     * The status property
     */
    private String status;
    /**
     * The valor property
     */
    private UntypedNode valor;
    /**
     * Instantiates a new {@link LancamentoERPDto} and sets the default values.
     */
    public LancamentoERPDto() {
        this.setAdditionalData(new HashMap<>());
    }
    /**
     * Creates a new instance of the appropriate class based on discriminator value
     * @param parseNode The parse node to use to read the discriminator value and create the object
     * @return a {@link LancamentoERPDto}
     */
    @jakarta.annotation.Nonnull
    public static LancamentoERPDto createFromDiscriminatorValue(@jakarta.annotation.Nonnull final ParseNode parseNode) {
        Objects.requireNonNull(parseNode);
        return new LancamentoERPDto();
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
     * Gets the cliente property value. The cliente property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getCliente() {
        return this.cliente;
    }
    /**
     * Gets the data property value. The data property
     * @return a {@link OffsetDateTime}
     */
    @jakarta.annotation.Nullable
    public OffsetDateTime getData() {
        return this.data;
    }
    /**
     * Gets the descricao property value. The descricao property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getDescricao() {
        return this.descricao;
    }
    /**
     * The deserialization information for the current model
     * @return a {@link Map<String, java.util.function.Consumer<ParseNode>>}
     */
    @jakarta.annotation.Nonnull
    public Map<String, java.util.function.Consumer<ParseNode>> getFieldDeserializers() {
        final HashMap<String, java.util.function.Consumer<ParseNode>> deserializerMap = new HashMap<String, java.util.function.Consumer<ParseNode>>(9);
        deserializerMap.put("cliente", (n) -> { this.setCliente(n.getStringValue()); });
        deserializerMap.put("data", (n) -> { this.setData(n.getOffsetDateTimeValue()); });
        deserializerMap.put("descricao", (n) -> { this.setDescricao(n.getStringValue()); });
        deserializerMap.put("fornecedor", (n) -> { this.setFornecedor(n.getStringValue()); });
        deserializerMap.put("id", (n) -> { this.setId(n.getStringValue()); });
        deserializerMap.put("numeroDocumento", (n) -> { this.setNumeroDocumento(n.getStringValue()); });
        deserializerMap.put("referencia", (n) -> { this.setReferencia(n.getStringValue()); });
        deserializerMap.put("status", (n) -> { this.setStatus(n.getStringValue()); });
        deserializerMap.put("valor", (n) -> { this.setValor(n.getObjectValue(UntypedNode::createFromDiscriminatorValue)); });
        return deserializerMap;
    }
    /**
     * Gets the fornecedor property value. The fornecedor property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getFornecedor() {
        return this.fornecedor;
    }
    /**
     * Gets the id property value. The id property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getId() {
        return this.id;
    }
    /**
     * Gets the numeroDocumento property value. The numeroDocumento property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getNumeroDocumento() {
        return this.numeroDocumento;
    }
    /**
     * Gets the referencia property value. The referencia property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getReferencia() {
        return this.referencia;
    }
    /**
     * Gets the status property value. The status property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getStatus() {
        return this.status;
    }
    /**
     * Gets the valor property value. The valor property
     * @return a {@link UntypedNode}
     */
    @jakarta.annotation.Nullable
    public UntypedNode getValor() {
        return this.valor;
    }
    /**
     * Serializes information the current object
     * @param writer Serialization writer to use to serialize this model
     */
    public void serialize(@jakarta.annotation.Nonnull final SerializationWriter writer) {
        Objects.requireNonNull(writer);
        writer.writeStringValue("cliente", this.getCliente());
        writer.writeOffsetDateTimeValue("data", this.getData());
        writer.writeStringValue("descricao", this.getDescricao());
        writer.writeStringValue("fornecedor", this.getFornecedor());
        writer.writeStringValue("id", this.getId());
        writer.writeStringValue("numeroDocumento", this.getNumeroDocumento());
        writer.writeStringValue("referencia", this.getReferencia());
        writer.writeStringValue("status", this.getStatus());
        writer.writeObjectValue("valor", this.getValor());
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
     * Sets the cliente property value. The cliente property
     * @param value Value to set for the cliente property.
     */
    public void setCliente(@jakarta.annotation.Nullable final String value) {
        this.cliente = value;
    }
    /**
     * Sets the data property value. The data property
     * @param value Value to set for the data property.
     */
    public void setData(@jakarta.annotation.Nullable final OffsetDateTime value) {
        this.data = value;
    }
    /**
     * Sets the descricao property value. The descricao property
     * @param value Value to set for the descricao property.
     */
    public void setDescricao(@jakarta.annotation.Nullable final String value) {
        this.descricao = value;
    }
    /**
     * Sets the fornecedor property value. The fornecedor property
     * @param value Value to set for the fornecedor property.
     */
    public void setFornecedor(@jakarta.annotation.Nullable final String value) {
        this.fornecedor = value;
    }
    /**
     * Sets the id property value. The id property
     * @param value Value to set for the id property.
     */
    public void setId(@jakarta.annotation.Nullable final String value) {
        this.id = value;
    }
    /**
     * Sets the numeroDocumento property value. The numeroDocumento property
     * @param value Value to set for the numeroDocumento property.
     */
    public void setNumeroDocumento(@jakarta.annotation.Nullable final String value) {
        this.numeroDocumento = value;
    }
    /**
     * Sets the referencia property value. The referencia property
     * @param value Value to set for the referencia property.
     */
    public void setReferencia(@jakarta.annotation.Nullable final String value) {
        this.referencia = value;
    }
    /**
     * Sets the status property value. The status property
     * @param value Value to set for the status property.
     */
    public void setStatus(@jakarta.annotation.Nullable final String value) {
        this.status = value;
    }
    /**
     * Sets the valor property value. The valor property
     * @param value Value to set for the valor property.
     */
    public void setValor(@jakarta.annotation.Nullable final UntypedNode value) {
        this.valor = value;
    }
}
