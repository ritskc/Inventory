import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  isLoggedIn: boolean = false;
  redirectUrl: string = '';

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  login(loginname: string, password: string): Observable<User> {
    return this.apiService.get<User>(`${ this.configService.Settings.apiServerHost }/${ this.configService.Settings.usersUri }/${ loginname }`)
  }

  logout(): void {
    this.isLoggedIn = false;
  }
}
