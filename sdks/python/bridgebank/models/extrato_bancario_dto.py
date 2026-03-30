from __future__ import annotations
import datetime
from collections.abc import Callable
from dataclasses import dataclass, field
from kiota_abstractions.serialization import AdditionalDataHolder, Parsable, ParseNode, SerializationWriter
from typing import Any, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .transacao_dto import TransacaoDto

@dataclass
class ExtratoBancarioDto(AdditionalDataHolder, Parsable):
    # Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.
    additional_data: dict[str, Any] = field(default_factory=dict)

    # The banco property
    banco: Optional[str] = None
    # The dataFim property
    data_fim: Optional[datetime.datetime] = None
    # The dataInicio property
    data_inicio: Optional[datetime.datetime] = None
    # The numeroConta property
    numero_conta: Optional[str] = None
    # The transacoes property
    transacoes: Optional[list[TransacaoDto]] = None
    
    @staticmethod
    def create_from_discriminator_value(parse_node: ParseNode) -> ExtratoBancarioDto:
        """
        Creates a new instance of the appropriate class based on discriminator value
        param parse_node: The parse node to use to read the discriminator value and create the object
        Returns: ExtratoBancarioDto
        """
        if parse_node is None:
            raise TypeError("parse_node cannot be null.")
        return ExtratoBancarioDto()
    
    def get_field_deserializers(self,) -> dict[str, Callable[[ParseNode], None]]:
        """
        The deserialization information for the current model
        Returns: dict[str, Callable[[ParseNode], None]]
        """
        from .transacao_dto import TransacaoDto

        from .transacao_dto import TransacaoDto

        fields: dict[str, Callable[[Any], None]] = {
            "banco": lambda n : setattr(self, 'banco', n.get_str_value()),
            "dataFim": lambda n : setattr(self, 'data_fim', n.get_datetime_value()),
            "dataInicio": lambda n : setattr(self, 'data_inicio', n.get_datetime_value()),
            "numeroConta": lambda n : setattr(self, 'numero_conta', n.get_str_value()),
            "transacoes": lambda n : setattr(self, 'transacoes', n.get_collection_of_object_values(TransacaoDto)),
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
        writer.write_str_value("banco", self.banco)
        writer.write_datetime_value("dataFim", self.data_fim)
        writer.write_datetime_value("dataInicio", self.data_inicio)
        writer.write_str_value("numeroConta", self.numero_conta)
        writer.write_collection_of_object_values("transacoes", self.transacoes)
        writer.write_additional_data_value(self.additional_data)
    

