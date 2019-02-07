### Azure Industrial IoT OPC Unified Architecture (OPC UA) Certificate Management Service

This article gives an overview about the OPC UA Certificate Management Service and the core microservice called OPCVault.

## Overview

OPC UA Applications use Application Instance Certificates to provide application level security. 
A secure connection is established using Asymmetric Cryptography, for which the application 
certificates provide the public and private key pair. 
The certificates can be self-signed or can be signed by a Certificate Authority (CA).

An OPC UA Application has a list of trusted certificates which represent the applications it trusts. 
These certificates can be self-signed, can be signed by a CA or can be a Root-CA or Sub-CA themselves. 
If a trusted certificate is part of a larger certificate chain, the application trusts all certificates 
which chain up to the certificate in the trust list, as long as the full certificate chain can be validated.

The major difference between trusting self-signed certificates and trusting a CA certificate 
is the installation effort required to deploy and maintain trust and the additional effort 
to host a company specific CA. 

In order to distribute trust for self signed certificates for n Servers with a single client 
application, all n Server Application Certificates have to be installed on the Client 
Application trust list and the Client Application Certificate must be installed on all 
Server Application trust lists. This administrative effort is quite a burden and adds 
even more when certificate lifetimes have to be considered and certificates are renewed.

The use of a company specific CA can greatly simplify the management of trust with 
multiple Servers and Clients. In this case the administrator generates a CA signed 
Application Instance certificate once for every Client and Server that is used. 
In addition, the CA Certificate is installed in every Application trust list, 
on all servers and clients. With this approach only expired Certificates need to 
be renewed and to be replaced for the affected applications.

Azure Industrial IoT OPC UA Certificate Management Service is our solution to 
manage a company specific CA for OPC UA applications which is based on the OPC Vault microservice.

OPC Vault provides a micro service to host a company specific CA in a secure 
cloud, backed by Azure AD secured services with Azure Key Vault with HSM, 
Cosmos DB and optionally also IoT Hub as application store.

The OPC Vault micro service is designed to support role based workflow where OT 
personal requests signed application certificates and where security 
administrators and approvers with signing rights in Azure Key Vault 
approve or reject these requests.

For compatibility with existing OPC UA GDS based OT solutions OPC Vault 
supports a micro service backed edge module which implements the 
OPC UA Global Discovery Server interface to distribute certificates 
and trust lists according to the Part 12 specification. 
However, as of our knowledge, this GDS server interface is not widely 
used yet and has yet limited functionality (Reader role). [On demand, we will 
improve the experience on customer request (*)](#Yet-Unsupported-features).

## Architecture

The architecture is based on a the OPC Vault microservice with an OPC Vault 
edge module for the factory network and an OPC Vault sample UX to control the workflow:

![OPCVault Architecture](opcvaultarchitecture.png "OPCVault architecture")


## OPC Vault Microservice

The OPC Vault microservice consists of the following interfaces to implement 
the workflow to distribute and manage a company specific CA for OPC UA Applications:

### Application 
- An “OPC UA Application” can be a server or a client or both. OPC Vault serves in this 
case as an application registration authority. 
- Beside the basic operations to register, 
update and unregister applications there are also interfaces to find and query 
for applications with search expressions. 
- The certificate requests need to reference 
a valid application in order to process a request and to issue a signed certificate 
with all OPC UA specific extensions. 
- The application service is either backed by a CosmosDB 
database or the [OpcTwin device registry (*)](#Yet-Unsupported-features), depending on the customer configuration.

### CertificateGroup
- A certificate group is an entity which stores a root CA or a sub CA certificate 
including the private key to sign certificates. 
- The RSA key length, the SHA-2 hash length 
and the lifetimes are configurable for both Issuer CA and signed application certificates. 
- Multiple groups can be hosted in a single service to allow for future extensions with https, 
user or ECC algorithm certificate groups [(*)](#Yet-Unsupported-features). 
- The CA certificates are stored in Azure Key Vault backed with FIPS 140-2 Level 2 Hardware Security Modules (HSM). 
The private key never leaves the secure storage because signing is done 
by an AzureAD secured Key Vault operation. 
- The CA certificates can be renewed over time and 
remain still in safe storage due to Key Vault history. 
- The revocation list for each CA certificate is also stored in Key Vault as a secret. 
Once an application is unregistered, the application certificate is also revoked in the CRL by an administrator.
- Batched and single certificate revocation is supported.

### CertificateRequest
A certificate request implements the workflow to generate a new key pair or a signed certificate using a CSR for an OPC UA Application. 
- The request is stored in a database with accompanying information like the Subject or a “Certificate Signing Request” (CSR) and a reference to the OPC UA Application. 
- The business logic in the service validates the request against the information stored in the application database. 
For example the application Uri in the database must match the application Uri in the CSR.
- A security administrator with signing rights (Approver role) approves or rejects the request. If the request is approved, a new key pair and/or a signed certificate are generated. The new private key is securely stored in KeyVault while the new signed public certificate is stored in the Certificate Request database.
- The requester can poll the request status until it is approved or revoked. If the request was approved, the private key and the certificate can be downloaded and installed in the certificate store of the OPC UA application.
- The requestor can now Accept the request to delete unnecessary information from the request database. 

Over the lifetime of a signed certificate an application might be deleted or a key might become compromised. In such a case, a CA manager can:
- Delete an application, which at the same time deletes also all pending and approved certificate requests of the app. 
- Another option is to delete just the single certificate request if only a key is renewed or compromised.
- Now compromised Approved and Accepted certificate requests are marked as deleted.
- A manager may regularly execute a renewal of the Issuer CA CRL. At the renewal time all the deleted Certificate Requests are revoked and the certificate serial numbers are added to the CRL revocation list. Revoked certificate requests are marked as revoked.
- In urgent events, single certificate requests can be revoked, too.
- Finally the updated CRLs are available for distribution to the participating OPC UA clients and servers.

See swagger documentation of the OPC Vault Microservice for more information on the API.

## OPC Vault Module
To support a factory network Global Discovery Server the OPC Vault module can be deployed on the edge, 
execute as a local .Net Core application or can be started in a docker image. 
Due to a lack of Auth2 authentication support in the current OPC UA .Net Standard stack, 
the functionality of the OPC Vault edge module is limited to a Reader role, because a user cannot be 
impersonated from the edge module to the micro service using the OPC UA GDS standard interface. 
Only operations which do not require the Write, Manage or Sign role are permitted at this point[(*)](#Yet-Unsupported-features). 

## Yet Unsupported features

**(*)** not supported yet, please open issues to request support for new features.
