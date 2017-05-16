# Web service models

* Add a "ApiModel" suffix to the models in this folder. This allows to
  distinguish these classes from the SDK and from the models used in the
  service layer.
* Hard code JSON property names using an explicit JsonProperty attribute.
* Do not reference classes from the Azure IoT SDK (or other SDKs).
