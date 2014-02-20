Namespace Cryptography

    ''' <summary>
    ''' SAML Actions
    ''' </summary>
    Public Enum SAMLAction
        ''' <summary>
        ''' Request
        ''' </summary>
        SAMLRequest
        ''' <summary>
        ''' Response
        ''' </summary>
        SAMLResponse
    End Enum

    Public Class Saml20Constants
        ''' <summary>
        ''' SAML Version
        ''' </summary>
        Public Const Version As String = "2.0"

        ''' <summary>
        ''' The XML namespace of the SAML 2.0 assertion schema.
        ''' </summary>
        Public Const ASSERTION As String = "urn:oasis:names:tc:SAML:2.0:assertion"

        ''' <summary>
        ''' The XML namespace of the SAML 2.0 protocol schema
        ''' </summary>
        Public Const PROTOCOL As String = "urn:oasis:names:tc:SAML:2.0:protocol"

        ''' <summary>
        ''' The XML namespace of the SAML 2.0 metadata schema
        ''' </summary>
        Public Const METADATA As String = "urn:oasis:names:tc:SAML:2.0:metadata"

        ''' <summary>
        ''' All the namespaces defined and reserved by the SAML 2.0 standard
        ''' </summary>
        Public Shared ReadOnly SAML_NAMESPACES As String() = New String() {ASSERTION, PROTOCOL, METADATA}

        ''' <summary>
        ''' The XML namespace of XmlDSig
        ''' </summary>
        Public Const XMLDSIG As String = "http://www.w3.org/2000/09/xmldsig#"

        ''' <summary>
        ''' The XML namespace of XmlEnc
        ''' </summary>
        Public Const XENC As String = "http://www.w3.org/2001/04/xmlenc#"

        ''' <summary>
        ''' The default value of the Format property for a NameID element
        ''' </summary>
        Public Const DEFAULTNAMEIDFORMAT As String = "urn:oasis:names:tc:SAML:1.0:nameid-format:unspecified"

        ''' <summary>
        ''' The mime type that must be used when publishing a metadata document.
        ''' </summary>
        Public Const METADATA_MIMETYPE As String = "application/samlmetadata+xml"

        ''' <summary>
        ''' A mandatory prefix for translating arbitrary saml2.0 claim names to saml1.1 attributes
        ''' </summary>
        Public Const DKSAML20_CLAIMTYPE_PREFIX As String = "dksaml20/"

        ''' <summary>
        ''' Formats of nameidentifiers
        ''' </summary>
        Public NotInheritable Class NameIdentifierFormats
            Private Sub New()
            End Sub
            ''' <summary>
            ''' urn for Unspecified name identifier format
            ''' </summary>
            Public Const Unspecified As String = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified"
            ''' <summary>
            ''' urn for Email name identifier format
            ''' </summary>
            Public Const Email As String = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress"
            ''' <summary>
            ''' urn for X509SubjectName name identifier format
            ''' </summary>
            Public Const X509SubjectName As String = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"
            ''' <summary>
            ''' urn for Windows name identifier format
            ''' </summary>
            Public Const Windows As String = "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName"
            ''' <summary>
            ''' urn for Kerberos name identifier format
            ''' </summary>
            Public Const Kerberos As String = "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos"
            ''' <summary>
            ''' urn for Entity name identifier format
            ''' </summary>
            Public Const Entity As String = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity"
            ''' <summary>
            ''' urn for Persistent name identifier format
            ''' </summary>
            Public Const Persistent As String = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent"
            ''' <summary>
            ''' urn for Transient name identifier format
            ''' </summary>
            Public Const Transient As String = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient"
        End Class

        ''' <summary>
        ''' Protocol bindings
        ''' </summary>
        Public NotInheritable Class ProtocolBindings
            Private Sub New()
            End Sub
            ''' <summary>
            ''' HTTP Redirect protocol binding
            ''' </summary>
            Public Const HTTP_Redirect As String = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect"
            ''' <summary>
            ''' HTTP Post protocol binding
            ''' </summary>
            Public Const HTTP_Post As String = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST"
            ''' <summary>
            ''' HTTP Artifact protocol binding
            ''' </summary>
            Public Const HTTP_Artifact As String = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact"
            ''' <summary>
            ''' HTTP SOAP  protocol binding
            ''' </summary>
            Public Const HTTP_SOAP As String = "urn:oasis:names:tc:SAML:2.0:bindings:SOAP"
        End Class

        ''' <summary>
        ''' Subject confirmation methods
        ''' </summary>
        Public NotInheritable Class SubjectConfirmationMethods
            Private Sub New()
            End Sub
            ''' <summary>
            ''' Holder of key confirmation method
            ''' </summary>
            Public Const HolderOfKey As String = "urn:oasis:names:tc:SAML:2.0:cm:holder-of-key"
        End Class

        ''' <summary>
        ''' Logout reasons
        ''' </summary>
        Public NotInheritable Class Reasons
            Private Sub New()
            End Sub
            ''' <summary>
            ''' Specifies that the message is being sent because the principal wishes to terminate the indicated session.
            ''' </summary>
            Public Const User As String = "urn:oasis:names:tc:SAML:2.0:logout:user"
            ''' <summary>
            ''' Specifies that the message is being sent because an administrator wishes to terminate the indicated
            ''' session for that principal.
            ''' </summary>
            Public Const Admin As String = "urn:oasis:names:tc:SAML:2.0:logout:admin"
        End Class

        ''' <summary>
        ''' Status codes
        ''' </summary>
        Public NotInheritable Class StatusCodes
            Private Sub New()
            End Sub
            ''' <summary>
            ''' The request succeeded.
            ''' </summary>
            Public Const Success As String = "urn:oasis:names:tc:SAML:2.0:status:Success"
            ''' <summary>
            ''' The request could not be performed due to an error on the part of the requester.
            ''' </summary>
            Public Const Requester As String = "urn:oasis:names:tc:SAML:2.0:status:Requester"
            ''' <summary>
            ''' The request could not be performed due to an error on the part of the SAML responder or SAML authority.
            ''' </summary>
            Public Const Responder As String = "urn:oasis:names:tc:SAML:2.0:status:Responder"
            ''' <summary>
            ''' The SAML responder could not process the request because the version of the request message was incorrect.
            ''' </summary>
            Public Const VersionMismatch As String = "urn:oasis:names:tc:SAML:2.0:status:VersionMismatch"
            ''' <summary>
            ''' The responding provider was unable to successfully authenticate the principal.
            ''' </summary>
            Public Const AuthnFailed As String = "urn:oasis:names:tc:SAML:2.0:status:AuthnFailed"
            ''' <summary>
            ''' Unexpected or invalid content was encountered within a &lt;saml:Attribute&gt; or &lt;saml:AttributeValue&gt; element.
            ''' </summary>
            Public Const InvalidAttrNameOrValue As String = "urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue"
            ''' <summary>
            ''' The responding provider cannot or will not support the requested name identifier policy.
            ''' </summary>
            Public Const InvalidNameIdPolicy As String = "urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy"
            ''' <summary>
            ''' The specified authentication context requirements cannot be met by the responder.
            ''' </summary>
            Public Const NoAuthnContext As String = "urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext"
            ''' <summary>
            ''' Used by an intermediary to indicate that none of the supported identity provider &lt;Loc&gt; elements in an
            ''' &lt;IDPList&gt; can be resolved or that none of the supported identity providers are available.
            ''' </summary>
            Public Const NoAvailableIDP As String = "urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP"
            ''' <summary>
            ''' Indicates the responding provider cannot authenticate the principal passively, as has been requested.
            ''' </summary>
            Public Const NoPassive As String = "urn:oasis:names:tc:SAML:2.0:status:NoPassive"
            ''' <summary>
            ''' Used by an intermediary to indicate that none of the identity providers in an &lt;IDPList&gt; are
            ''' supported by the intermediary.
            ''' </summary>
            Public Const NoSupportedIDP As String = "urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP"
            ''' <summary>
            ''' Used by a session authority to indicate to a session participant that it was not able to propagate logout
            ''' to all other session participants.
            ''' </summary>
            Public Const PartialLogout As String = "urn:oasis:names:tc:SAML:2.0:status:PartialLogout"
            ''' <summary>
            ''' Indicates that a responding provider cannot authenticate the principal directly and is not permitted to
            ''' proxy the request further.
            ''' </summary>
            Public Const ProxyCountExceeded As String = "urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded"
            ''' <summary>
            ''' The SAML responder or SAML authority is able to process the request but has chosen not to respond.
            ''' This status code MAY be used when there is concern about the security context of the request
            ''' message or the sequence of request messages received from a particular requester.
            ''' </summary>
            Public Const RequestDenied As String = "urn:oasis:names:tc:SAML:2.0:status:RequestDenied"
            ''' <summary>
            ''' The SAML responder or SAML authority does not support the request.
            ''' </summary>
            Public Const RequestUnsupported As String = "urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported"
            ''' <summary>
            ''' The SAML responder cannot process any requests with the protocol version specified in the request.
            ''' </summary>
            Public Const RequestVersionDeprecated As String = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated"
            ''' <summary>
            ''' The SAML responder cannot process the request because the protocol version specified in the
            ''' request message is a major upgrade from the highest protocol version supported by the responder.
            ''' </summary>
            Public Const RequestVersionTooHigh As String = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh"
            ''' <summary>
            ''' The SAML responder cannot process the request because the protocol version specified in the
            ''' request message is too low.
            ''' </summary>
            Public Const RequestVersionTooLow As String = "urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow"
            ''' <summary>
            ''' The resource value provided in the request message is invalid or unrecognized.
            ''' </summary>
            Public Const ResourceNotRecognized As String = "urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized"
            ''' <summary>
            ''' The response message would contain more elements than the SAML responder is able to return.
            ''' </summary>
            Public Const TooManyResponses As String = "urn:oasis:names:tc:SAML:2.0:status:TooManyResponses"
            ''' <summary>
            ''' An entity that has no knowledge of a particular attribute profile has been presented with an attribute
            ''' drawn from that profile.
            ''' </summary>
            Public Const UnknownAttrProfile As String = "urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile"
            ''' <summary>
            ''' The responding provider does not recognize the principal specified or implied by the request.
            ''' </summary>
            Public Const UnknownPrincipal As String = "urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal"
            ''' <summary>
            ''' The SAML responder cannot properly fulfill the request using the protocol binding specified in the
            ''' request.
            ''' </summary>
            Public Const UnsupportedBinding As String = "urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding"
        End Class
    End Class
End Namespace