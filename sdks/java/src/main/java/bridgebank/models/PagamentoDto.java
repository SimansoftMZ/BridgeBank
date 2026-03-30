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
public class PagamentoDto implements AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    private Map<String, Object> additionalData;
    /**
     * The bancoBeneficiario property
     */
    private String bancoBeneficiario;
    /**
     * The beneficiario property
     */
    private String beneficiario;
    /**
     * The contaBeneficiario property
     */
    private String contaBeneficiario;
    /**
     * The dataPagamento property
     */
    private OffsetDateTime dataPagamento;
    /**
     * The descricao property
     */
    private String descricao;
    /**
     * The id property
     */
    private String id;
    /**
     * The referencia property
     */
    private String referencia;
    /**
     * The tipo property
     */
    private String tipo;
    /**
     * The valor property
     */
    private UntypedNode valor;
    /**
     * Instantiates a new {@link PagamentoDto} and sets the default values.
     */
    public PagamentoDto() {
        this.setAdditionalData(new HashMap<>());
    }
    /**
     * Creates a new instance of the appropriate class based on discriminator value
     * @param parseNode The parse node to use to read the discriminator value and create the object
     * @return a {@link PagamentoDto}
     */
    @jakarta.annotation.Nonnull
    public static PagamentoDto createFromDiscriminatorValue(@jakarta.annotation.Nonnull final ParseNode parseNode) {
        Objects.requireNonNull(parseNode);
        return new PagamentoDto();
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
     * Gets the bancoBeneficiario property value. The bancoBeneficiario property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getBancoBeneficiario() {
        return this.bancoBeneficiario;
    }
    /**
     * Gets the beneficiario property value. The beneficiario property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getBeneficiario() {
        return this.beneficiario;
    }
    /**
     * Gets the contaBeneficiario property value. The contaBeneficiario property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getContaBeneficiario() {
        return this.contaBeneficiario;
    }
    /**
     * Gets the dataPagamento property value. The dataPagamento property
     * @return a {@link OffsetDateTime}
     */
    @jakarta.annotation.Nullable
    public OffsetDateTime getDataPagamento() {
        return this.dataPagamento;
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
        deserializerMap.put("bancoBeneficiario", (n) -> { this.setBancoBeneficiario(n.getStringValue()); });
        deserializerMap.put("beneficiario", (n) -> { this.setBeneficiario(n.getStringValue()); });
        deserializerMap.put("contaBeneficiario", (n) -> { this.setContaBeneficiario(n.getStringValue()); });
        deserializerMap.put("dataPagamento", (n) -> { this.setDataPagamento(n.getOffsetDateTimeValue()); });
        deserializerMap.put("descricao", (n) -> { this.setDescricao(n.getStringValue()); });
        deserializerMap.put("id", (n) -> { this.setId(n.getStringValue()); });
        deserializerMap.put("referencia", (n) -> { this.setReferencia(n.getStringValue()); });
        deserializerMap.put("tipo", (n) -> { this.setTipo(n.getStringValue()); });
        deserializerMap.put("valor", (n) -> { this.setValor(n.getObjectValue(UntypedNode::createFromDiscriminatorValue)); });
        return deserializerMap;
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
     * Gets the referencia property value. The referencia property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getReferencia() {
        return this.referencia;
    }
    /**
     * Gets the tipo property value. The tipo property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getTipo() {
        return this.tipo;
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
        writer.writeStringValue("bancoBeneficiario", this.getBancoBeneficiario());
        writer.writeStringValue("beneficiario", this.getBeneficiario());
        writer.writeStringValue("contaBeneficiario", this.getContaBeneficiario());
        writer.writeOffsetDateTimeValue("dataPagamento", this.getDataPagamento());
        writer.writeStringValue("descricao", this.getDescricao());
        writer.writeStringValue("id", this.getId());
        writer.writeStringValue("referencia", this.getReferencia());
        writer.writeStringValue("tipo", this.getTipo());
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
     * Sets the bancoBeneficiario property value. The bancoBeneficiario property
     * @param value Value to set for the bancoBeneficiario property.
     */
    public void setBancoBeneficiario(@jakarta.annotation.Nullable final String value) {
        this.bancoBeneficiario = value;
    }
    /**
     * Sets the beneficiario property value. The beneficiario property
     * @param value Value to set for the beneficiario property.
     */
    public void setBeneficiario(@jakarta.annotation.Nullable final String value) {
        this.beneficiario = value;
    }
    /**
     * Sets the contaBeneficiario property value. The contaBeneficiario property
     * @param value Value to set for the contaBeneficiario property.
     */
    public void setContaBeneficiario(@jakarta.annotation.Nullable final String value) {
        this.contaBeneficiario = value;
    }
    /**
     * Sets the dataPagamento property value. The dataPagamento property
     * @param value Value to set for the dataPagamento property.
     */
    public void setDataPagamento(@jakarta.annotation.Nullable final OffsetDateTime value) {
        this.dataPagamento = value;
    }
    /**
     * Sets the descricao property value. The descricao property
     * @param value Value to set for the descricao property.
     */
    public void setDescricao(@jakarta.annotation.Nullable final String value) {
        this.descricao = value;
    }
    /**
     * Sets the id property value. The id property
     * @param value Value to set for the id property.
     */
    public void setId(@jakarta.annotation.Nullable final String value) {
        this.id = value;
    }
    /**
     * Sets the referencia property value. The referencia property
     * @param value Value to set for the referencia property.
     */
    public void setReferencia(@jakarta.annotation.Nullable final String value) {
        this.referencia = value;
    }
    /**
     * Sets the tipo property value. The tipo property
     * @param value Value to set for the tipo property.
     */
    public void setTipo(@jakarta.annotation.Nullable final String value) {
        this.tipo = value;
    }
    /**
     * Sets the valor property value. The valor property
     * @param value Value to set for the valor property.
     */
    public void setValor(@jakarta.annotation.Nullable final UntypedNode value) {
        this.valor = value;
    }
}
