from __future__ import annotations
from collections.abc import Callable
from kiota_abstractions.base_request_builder import BaseRequestBuilder
from kiota_abstractions.get_path_parameters import get_path_parameters
from kiota_abstractions.request_adapter import RequestAdapter
from typing import Any, Optional, TYPE_CHECKING, Union

if TYPE_CHECKING:
    from .categorias.categorias_request_builder import CategoriasRequestBuilder
    from .tipos_correspondencia.tipos_correspondencia_request_builder import TiposCorrespondenciaRequestBuilder
    from .tipos_transacao.tipos_transacao_request_builder import TiposTransacaoRequestBuilder

class EnumsRequestBuilder(BaseRequestBuilder):
    """
    Builds and executes requests for operations under /api/enums
    """
    def __init__(self,request_adapter: RequestAdapter, path_parameters: Union[str, dict[str, Any]]) -> None:
        """
        Instantiates a new EnumsRequestBuilder and sets the default values.
        param path_parameters: The raw url or the url-template parameters for the request.
        param request_adapter: The request adapter to use to execute the requests.
        Returns: None
        """
        super().__init__(request_adapter, "{+baseurl}/api/enums", path_parameters)
    
    @property
    def categorias(self) -> CategoriasRequestBuilder:
        """
        The categorias property
        """
        from .categorias.categorias_request_builder import CategoriasRequestBuilder

        return CategoriasRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def tipos_correspondencia(self) -> TiposCorrespondenciaRequestBuilder:
        """
        The tiposCorrespondencia property
        """
        from .tipos_correspondencia.tipos_correspondencia_request_builder import TiposCorrespondenciaRequestBuilder

        return TiposCorrespondenciaRequestBuilder(self.request_adapter, self.path_parameters)
    
    @property
    def tipos_transacao(self) -> TiposTransacaoRequestBuilder:
        """
        The tiposTransacao property
        """
        from .tipos_transacao.tipos_transacao_request_builder import TiposTransacaoRequestBuilder

        return TiposTransacaoRequestBuilder(self.request_adapter, self.path_parameters)
    

