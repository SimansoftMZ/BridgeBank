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
public class ExtratoBancarioDto implements AdditionalDataHolder, Parsable {
    /**
     * Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
     */
    private Map<String, Object> additionalData;
    /**
     * The banco property
     */
    private String banco;
    /**
     * The dataFim property
     */
    private OffsetDateTime dataFim;
    /**
     * The dataInicio property
     */
    private OffsetDateTime dataInicio;
    /**
     * The numeroConta property
     */
    private String numeroConta;
    /**
     * The saldoFinal property
     */
    private UntypedNode saldoFinal;
    /**
     * The saldoInicial property
     */
    private UntypedNode saldoInicial;
    /**
     * The transacoes property
     */
    private java.util.List<TransacaoDto> transacoes;
    /**
     * Instantiates a new {@link ExtratoBancarioDto} and sets the default values.
     */
    public ExtratoBancarioDto() {
        this.setAdditionalData(new HashMap<>());
    }
    /**
     * Creates a new instance of the appropriate class based on discriminator value
     * @param parseNode The parse node to use to read the discriminator value and create the object
     * @return a {@link ExtratoBancarioDto}
     */
    @jakarta.annotation.Nonnull
    public static ExtratoBancarioDto createFromDiscriminatorValue(@jakarta.annotation.Nonnull final ParseNode parseNode) {
        Objects.requireNonNull(parseNode);
        return new ExtratoBancarioDto();
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
     * Gets the banco property value. The banco property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getBanco() {
        return this.banco;
    }
    /**
     * Gets the dataFim property value. The dataFim property
     * @return a {@link OffsetDateTime}
     */
    @jakarta.annotation.Nullable
    public OffsetDateTime getDataFim() {
        return this.dataFim;
    }
    /**
     * Gets the dataInicio property value. The dataInicio property
     * @return a {@link OffsetDateTime}
     */
    @jakarta.annotation.Nullable
    public OffsetDateTime getDataInicio() {
        return this.dataInicio;
    }
    /**
     * The deserialization information for the current model
     * @return a {@link Map<String, java.util.function.Consumer<ParseNode>>}
     */
    @jakarta.annotation.Nonnull
    public Map<String, java.util.function.Consumer<ParseNode>> getFieldDeserializers() {
        final HashMap<String, java.util.function.Consumer<ParseNode>> deserializerMap = new HashMap<String, java.util.function.Consumer<ParseNode>>(7);
        deserializerMap.put("banco", (n) -> { this.setBanco(n.getStringValue()); });
        deserializerMap.put("dataFim", (n) -> { this.setDataFim(n.getOffsetDateTimeValue()); });
        deserializerMap.put("dataInicio", (n) -> { this.setDataInicio(n.getOffsetDateTimeValue()); });
        deserializerMap.put("numeroConta", (n) -> { this.setNumeroConta(n.getStringValue()); });
        deserializerMap.put("saldoFinal", (n) -> { this.setSaldoFinal(n.getObjectValue(UntypedNode::createFromDiscriminatorValue)); });
        deserializerMap.put("saldoInicial", (n) -> { this.setSaldoInicial(n.getObjectValue(UntypedNode::createFromDiscriminatorValue)); });
        deserializerMap.put("transacoes", (n) -> { this.setTransacoes(n.getCollectionOfObjectValues(TransacaoDto::createFromDiscriminatorValue)); });
        return deserializerMap;
    }
    /**
     * Gets the numeroConta property value. The numeroConta property
     * @return a {@link String}
     */
    @jakarta.annotation.Nullable
    public String getNumeroConta() {
        return this.numeroConta;
    }
    /**
     * Gets the saldoFinal property value. The saldoFinal property
     * @return a {@link UntypedNode}
     */
    @jakarta.annotation.Nullable
    public UntypedNode getSaldoFinal() {
        return this.saldoFinal;
    }
    /**
     * Gets the saldoInicial property value. The saldoInicial property
     * @return a {@link UntypedNode}
     */
    @jakarta.annotation.Nullable
    public UntypedNode getSaldoInicial() {
        return this.saldoInicial;
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
        writer.writeStringValue("banco", this.getBanco());
        writer.writeOffsetDateTimeValue("dataFim", this.getDataFim());
        writer.writeOffsetDateTimeValue("dataInicio", this.getDataInicio());
        writer.writeStringValue("numeroConta", this.getNumeroConta());
        writer.writeObjectValue("saldoFinal", this.getSaldoFinal());
        writer.writeObjectValue("saldoInicial", this.getSaldoInicial());
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
     * Sets the banco property value. The banco property
     * @param value Value to set for the banco property.
     */
    public void setBanco(@jakarta.annotation.Nullable final String value) {
        this.banco = value;
    }
    /**
     * Sets the dataFim property value. The dataFim property
     * @param value Value to set for the dataFim property.
     */
    public void setDataFim(@jakarta.annotation.Nullable final OffsetDateTime value) {
        this.dataFim = value;
    }
    /**
     * Sets the dataInicio property value. The dataInicio property
     * @param value Value to set for the dataInicio property.
     */
    public void setDataInicio(@jakarta.annotation.Nullable final OffsetDateTime value) {
        this.dataInicio = value;
    }
    /**
     * Sets the numeroConta property value. The numeroConta property
     * @param value Value to set for the numeroConta property.
     */
    public void setNumeroConta(@jakarta.annotation.Nullable final String value) {
        this.numeroConta = value;
    }
    /**
     * Sets the saldoFinal property value. The saldoFinal property
     * @param value Value to set for the saldoFinal property.
     */
    public void setSaldoFinal(@jakarta.annotation.Nullable final UntypedNode value) {
        this.saldoFinal = value;
    }
    /**
     * Sets the saldoInicial property value. The saldoInicial property
     * @param value Value to set for the saldoInicial property.
     */
    public void setSaldoInicial(@jakarta.annotation.Nullable final UntypedNode value) {
        this.saldoInicial = value;
    }
    /**
     * Sets the transacoes property value. The transacoes property
     * @param value Value to set for the transacoes property.
     */
    public void setTransacoes(@jakarta.annotation.Nullable final java.util.List<TransacaoDto> value) {
        this.transacoes = value;
    }
}
