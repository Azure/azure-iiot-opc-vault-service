# Web service controllers

* Version controllers and models together.
* Always access external services through logic in the service layer.
* Consume Azure IoT SDK code through the service layer, i.e. do not reference
  classes from the Azure IoT SDK (or other SDKs).
