from __future__ import annotations
import datetime
from collections.abc import Callable
from dataclasses import dataclass, field
from kiota_abstractions.serialization import AdditionalDataHolder, Parsable, ParseNode, SerializationWriter
from typing import Any, Optional, TYPE_CHECKING, Union

@dataclass
class PagamentoDto(AdditionalDataHolder, Parsable):
    # Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
    additional_data: dict[str, Any] = field(default_factory=dict)

    # The bancoBeneficiario property
    banco_beneficiario: Optional[str] = None
    # The beneficiario property
    beneficiario: Optional[str] = None
    # The contaBeneficiario property
    conta_beneficiario: Optional[str] = None
    # The dataPagamento property
    data_pagamento: Optional[datetime.datetime] = None
    # The descricao property
    descricao: Optional[str] = None
    # The id property
    id: Optional[str] = None
    # The referencia property
    referencia: Optional[str] = None
    # The tipo property
    tipo: Optional[str] = None
    
    @staticmethod
    def create_from_discriminator_value(parse_node: ParseNode) -> PagamentoDto:
        """
        Creates a new instance of the appropriate class based on discriminator value
        param parse_node: The parse node to use to read the discriminator value and create the object
        Returns: PagamentoDto
        """
        if parse_node is None:
            raise TypeError("parse_node cannot be null.")
        return PagamentoDto()
    
    def get_field_deserializers(self,) -> dict[str, Callable[[ParseNode], None]]:
        """
        The deserialization information for the current model
        Returns: dict[str, Callable[[ParseNode], None]]
        """
        fields: dict[str, Callable[[Any], None]] = {
            "bancoBeneficiario": lambda n : setattr(self, 'banco_beneficiario', n.get_str_value()),
            "beneficiario": lambda n : setattr(self, 'beneficiario', n.get_str_value()),
            "contaBeneficiario": lambda n : setattr(self, 'conta_beneficiario', n.get_str_value()),
            "dataPagamento": lambda n : setattr(self, 'data_pagamento', n.get_datetime_value()),
            "descricao": lambda n : setattr(self, 'descricao', n.get_str_value()),
            "id": lambda n : setattr(self, 'id', n.get_str_value()),
            "referencia": lambda n : setattr(self, 'referencia', n.get_str_value()),
            "tipo": lambda n : setattr(self, 'tipo', n.get_str_value()),
        }
        return fields
    
    def serialize(self,writer: SerializationWriter) -> None:
        """
        Serializes information the current object
        param writer: Serialization writer to use to serialize this model
        Returns: None
        """
        if writer is None:
            raise TypeError("writer cannot be null.")
        writer.write_str_value("bancoBeneficiario", self.banco_beneficiario)
        writer.write_str_value("beneficiario", self.beneficiario)
        writer.write_str_value("contaBeneficiario", self.conta_beneficiario)
        writer.write_datetime_value("dataPagamento", self.data_pagamento)
        writer.write_str_value("descricao", self.descricao)
        writer.write_str_value("id", self.id)
        writer.write_str_value("referencia", self.referencia)
        writer.write_str_value("tipo", self.tipo)
        writer.write_additional_data_value(self.additional_data)
    

