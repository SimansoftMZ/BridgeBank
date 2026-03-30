from __future__ import annotations
from collections.abc import Callable
from kiota_abstractions.base_request_builder import BaseRequestBuilder
from kiota_abstractions.get_path_parameters import get_path_parameters
from kiota_abstractions.request_adapter import RequestAdapter
from typing import Any, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .bancos.bancos_request_builder import BancosRequestBuilder
    from .classificacao.classificacao_request_builder import ClassificacaoRequestBuilder
    from .enums.enums_request_builder import EnumsRequestBuilder
    from .extratos.extratos_request_builder import ExtratosRequestBuilder
    from .pagamentos.pagamentos_request_builder import PagamentosRequestBuilder
    from .reconciliacao.reconciliacao_request_builder import ReconciliacaoRequestBuilder

class ApiRequestBuilder(BaseRequestBuilder):
    """
    Builds and executes requests for operations under /api
    """
    def __init__(self,request_adapter: RequestAdapter, path_parameters: Union[str, dict[str, Any]]) -> None:
        """
        Instantiates a new ApiRequestBuilder and sets the default values.
        param path_parameters: The raw url or the url-template parameters for the request.
        param request_adapter: The request adapter to use to execute the requests.
        Returns: None
        """
        super().__init__(request_adapter, "{+baseurl}/api", path_parameters)
    
    @property
    def bancos(self) -> BancosRequestBuilder:
        """
        The bancos property
        """
        from .bancos.bancos_request_builder import BancosRequestBuilder

        return BancosRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def classificacao(self) -> ClassificacaoRequestBuilder:
        """
        The classificacao property
        """
        from .classificacao.classificacao_request_builder import ClassificacaoRequestBuilder

        return ClassificacaoRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def enums(self) -> EnumsRequestBuilder:
        """
        The enums property
        """
        from .enums.enums_request_builder import EnumsRequestBuilder

        return EnumsRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def extratos(self) -> ExtratosRequestBuilder:
        """
        The extratos property
        """
        from .extratos.extratos_request_builder import ExtratosRequestBuilder

        return ExtratosRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def pagamentos(self) -> PagamentosRequestBuilder:
        """
        The pagamentos property
        """
        from .pagamentos.pagamentos_request_builder import PagamentosRequestBuilder

        return PagamentosRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def reconciliacao(self) -> ReconciliacaoRequestBuilder:
        """
        The reconciliacao property
        """
        from .reconciliacao.reconciliacao_request_builder import ReconciliacaoRequestBuilder

        return ReconciliacaoRequestBuilder(self.request_adapter, self.path_parameters)
    

