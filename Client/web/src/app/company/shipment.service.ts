import { Injectable } from '@angular/core';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { Shipment } from '../models/shipment.model';
import { Observable } from 'rxjs';
import { MasterShipment } from '../models/master.shipment.model';

@Injectable({
  providedIn: 'root'
})
export class ShipmentService {

  constructor(private apiService: ApiService, private config: ConfigService) { }

  getAllShipments(companyId: number): Observable<Shipment[]> {
    return this.apiService.get<Shipment[]>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + '/' + companyId);
  }

  getAShipment(companyId: number, shipmentId: number): Observable<Shipment> {
    return this.apiService.get<Shipment>(this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + `/${ companyId }/` + shipmentId);
  }

  createShipment(shipment: Shipment) {
    if (shipment.id < 1) 
      return this.apiService.post(shipment, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
    else
      return this.apiService.put(shipment, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri + `/${ shipment.id }`);
  }

  deleteShipment(shipmentId: number) {
    return this.apiService.delete(shipmentId, this.config.Settings.apiServerHost + this.config.Settings.shipmentUri);
  }

  verifyShipment(shipment: Shipment) {
    return this.apiService.post(shipment, `${this.config.Settings.apiServerHost}/${this.config.Settings.shipmentUri}/verifyshipment/${shipment.id}`);
  }

  undoVerifyShipment(shipment: Shipment) {
    return this.apiService.post(shipment, `${this.config.Settings.apiServerHost}/${this.config.Settings.shipmentUri}/undoverifyshipment/${shipment.id}`);
  }

  allowScanning(shipment: Shipment) {
    return this.apiService.put(shipment, `${this.config.Settings.apiServerHost}/${this.config.Settings.shipmentUri}/allowscanning/${shipment.id}`);
  }

  autoScanning(shipment: Shipment) {
    return this.apiService.put(shipment, `${this.config.Settings.apiServerHost}/${this.config.Settings.shipmentUri}/autoscan/${shipment.id}`);
  }

  getLatestShipment(companyId: number, date: string): Observable<any> {
    return this.apiService.get(`${ this.config.Settings.apiServerHost }/${ this.config.Settings.entityTracker }/packing_slip/${ companyId }/${ date }`);
  }

  getAllMasterShipments(companyId: number): Observable<MasterShipment[]> {
    return this.apiService.get<MasterShipment[]>(this.config.Settings.apiServerHost + this.config.Settings.masterShipmentUri + '/' + companyId);
  }

  getAMasterShipments(companyId: number, masterShipmentId: number): Observable<MasterShipment> {
    return this.apiService.get<MasterShipment>(this.config.Settings.apiServerHost + this.config.Settings.masterShipmentUri + '/' + companyId + '/' + masterShipmentId);
  }

  getLatestMasterShipment(companyId: number, date: string): Observable<any> {
    return this.apiService.get(`${ this.config.Settings.apiServerHost }/${ this.config.Settings.entityTracker }/master_packing_slip/${ companyId }/${ date }`);
  }

  saveMasterShipment(masterShipment: MasterShipment) {
    if (masterShipment.id < 1) 
      return this.apiService.post(masterShipment, this.config.Settings.apiServerHost + this.config.Settings.masterShipmentUri);
    else
      return this.apiService.put(masterShipment, this.config.Settings.apiServerHost + this.config.Settings.masterShipmentUri + `/${ masterShipment.id }`);
  }

  removeMasterPackingSlip(masterShipmentId: number) {
    return this.apiService.delete(masterShipmentId, this.config.Settings.apiServerHost + this.config.Settings.masterShipmentUri);
  }
}
