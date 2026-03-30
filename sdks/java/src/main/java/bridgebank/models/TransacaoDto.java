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
public class TransacaoDto implements AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    private Map<String, Object> additionalData;
    /**
     * The beneficiario property
     */
    private String beneficiario;
    /**
     * The categoria property
     */
    private String categoria;
    /**
     * The confiancaClassificacao property
     */
    private UntypedNode confiancaClassificacao;
    /**
     * The contaBancaria property
     */
    private String contaBancaria;
    /**
     * The data property
     */
    private OffsetDateTime data;
    /**
     * The descricao property
     */
    private String descricao;
    /**
     * The documentoOrigem property
     */
    private String documentoOrigem;
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
     * Instantiates a new {@link TransacaoDto} and sets the default values.
     */
    public TransacaoDto() {
        this.setAdditionalData(new HashMap<>());
    }
    /**
     * Creates a new instance of the appropriate class based on discriminator value
     * @param parseNode The parse node to use to read the discriminator value and create the object
     * @return a {@link TransacaoDto}
     */
    @jakarta.annotation.Nonnull
    public static TransacaoDto createFromDiscriminatorValue(@jakarta.annotation.Nonnull final ParseNode parseNode) {
        Objects.requireNonNull(parseNode);
        return new TransacaoDto();
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
     * Gets the beneficiario property value. The beneficiario property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getBeneficiario() {
        return this.beneficiario;
    }
    /**
     * Gets the categoria property value. The categoria property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getCategoria() {
        return this.categoria;
    }
    /**
     * Gets the confiancaClassificacao property value. The confiancaClassificacao property
     * @return a {@link UntypedNode}
     */
    @jakarta.annotation.Nullable
    public UntypedNode getConfiancaClassificacao() {
        return this.confiancaClassificacao;
    }
    /**
     * Gets the contaBancaria property value. The contaBancaria property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getContaBancaria() {
        return this.contaBancaria;
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
     * Gets the documentoOrigem property value. The documentoOrigem property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getDocumentoOrigem() {
        return this.documentoOrigem;
    }
    /**
     * The deserialization information for the current model
     * @return a {@link Map<String, java.util.function.Consumer<ParseNode>>}
     */
    @jakarta.annotation.Nonnull
    public Map<String, java.util.function.Consumer<ParseNode>> getFieldDeserializers() {
        final HashMap<String, java.util.function.Consumer<ParseNode>> deserializerMap = new HashMap<String, java.util.function.Consumer<ParseNode>>(11);
        deserializerMap.put("beneficiario", (n) -> { this.setBeneficiario(n.getStringValue()); });
        deserializerMap.put("categoria", (n) -> { this.setCategoria(n.getStringValue()); });
        deserializerMap.put("confiancaClassificacao", (n) -> { this.setConfiancaClassificacao(n.getObjectValue(UntypedNode::createFromDiscriminatorValue)); });
        deserializerMap.put("contaBancaria", (n) -> { this.setContaBancaria(n.getStringValue()); });
        deserializerMap.put("data", (n) -> { this.setData(n.getOffsetDateTimeValue()); });
        deserializerMap.put("descricao", (n) -> { this.setDescricao(n.getStringValue()); });
        deserializerMap.put("documentoOrigem", (n) -> { this.setDocumentoOrigem(n.getStringValue()); });
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
        writer.writeStringValue("beneficiario", this.getBeneficiario());
        writer.writeStringValue("categoria", this.getCategoria());
        writer.writeObjectValue("confiancaClassificacao", this.getConfiancaClassificacao());
        writer.writeStringValue("contaBancaria", this.getContaBancaria());
        writer.writeOffsetDateTimeValue("data", this.getData());
        writer.writeStringValue("descricao", this.getDescricao());
        writer.writeStringValue("documentoOrigem", this.getDocumentoOrigem());
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
     * Sets the beneficiario property value. The beneficiario property
     * @param value Value to set for the beneficiario property.
     */
    public void setBeneficiario(@jakarta.annotation.Nullable final String value) {
        this.beneficiario = value;
    }
    /**
     * Sets the categoria property value. The categoria property
     * @param value Value to set for the categoria property.
     */
    public void setCategoria(@jakarta.annotation.Nullable final String value) {
        this.categoria = value;
    }
    /**
     * Sets the confiancaClassificacao property value. The confiancaClassificacao property
     * @param value Value to set for the confiancaClassificacao property.
     */
    public void setConfiancaClassificacao(@jakarta.annotation.Nullable final UntypedNode value) {
        this.confiancaClassificacao = value;
    }
    /**
     * Sets the contaBancaria property value. The contaBancaria property
     * @param value Value to set for the contaBancaria property.
     */
    public void setContaBancaria(@jakarta.annotation.Nullable final String value) {
        this.contaBancaria = value;
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
     * Sets the documentoOrigem property value. The documentoOrigem property
     * @param value Value to set for the documentoOrigem property.
     */
    public void setDocumentoOrigem(@jakarta.annotation.Nullable final String value) {
        this.documentoOrigem = value;
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
