#!/usr/bin/env pwsh

    [CmdletBinding()]
Param(
    [Parameter(Mandatory = $true)]
    [String]$TargetEnvironment,

    [Parameter(Mandatory = $false)]
    [Int]$ValidDays = 3600
)

$FilePrefix = "SigningCertificate-"

openssl req -x509 -newkey rsa:4096 -keyout "${FilePrefix}${TargetEnvironment}.key" -out "${FilePrefix}${TargetEnvironment}.pem" -days $ValidDays -subj "/C=CZ/ST=Czech Republic/L=Prague/O=RIDICS/OU=IT Department/CN=${TargetEnvironment}"

$NotUsableYet = @"
openssl ecparam -name secp521r1 -out "${FilePrefix}secp521r1.pem"
openssl ecparam -in "${FilePrefix}secp521r1.pem" -genkey -noout -out "${FilePrefix}${TargetEnvironment}.key"

openssl req -new -key "${FilePrefix}${TargetEnvironment}.key" -out "${FilePrefix}${TargetEnvironment}.csr" -subj "/C=CZ/ST=Czech Republic/L=Prague/O=RIDICS/OU=IT Department/CN=${TargetEnvironment}"

openssl x509 -req -days $ValidDays -in "${FilePrefix}${TargetEnvironment}.csr" -signkey "${FilePrefix}${TargetEnvironment}.key" -out "${FilePrefix}${TargetEnvironment}.pem"
"@

openssl pkcs12 -keysig -in "${FilePrefix}${TargetEnvironment}.pem" -inkey "${FilePrefix}${TargetEnvironment}.key" -export -out "${FilePrefix}${TargetEnvironment}.pfx"
