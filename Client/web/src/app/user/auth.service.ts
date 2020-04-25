import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, tap } from 'rxjs/operators';
import { ApiService } from '../common/services/api.service';
import { ConfigService } from '../config/config.service';
import { User } from '../models/user.model';
import { navItems } from '../_nav';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  isLoggedIn: boolean = false;
  redirectUrl: string = '';
  private navItems: any = navItems;

  constructor(private apiService: ApiService, private configService: ConfigService) { }

  login(loginname: string, password: string) {
    var user = new User();
    user.username = loginname;
    user.password = password;
    return this.apiService.post(user, this.configService.Settings.apiServerHost + this.configService.Settings.usersUri + '/authenticate');
  }

  logout(): void {
    this.isLoggedIn = false;
  }

  setPrivileges(privileges: any) {
    localStorage.setItem('privileges', JSON.stringify(privileges));
    if (!localStorage.getItem('menus')){
      localStorage.setItem('menus', JSON.stringify(navItems));
    }
    this.navItems = JSON.parse(localStorage.getItem('menus'));
    if (privileges.isSuperAdmin) {
      localStorage.setItem('filtered', JSON.stringify(this.navItems));
      var landingurl = this.navItems[0].children[0].url;
      localStorage.setItem('landingurl', landingurl);
      return;
    }

    this.navItems.forEach(navItem => {
      if (navItem.children && navItem.children.length > 0) {
        for(var i = 0; i < navItem.children.length; i++) {
          var x = privileges.userPriviledge.userMenus.findIndex(u => u.menu == navItem.children[i].name);
          if (x == -1) {
            navItem.children.splice(i, 1);
            i--;
          }
        }
      }
    });

    this.navItems = this.navItems.filter(n => n.children.length > 0);
    localStorage.setItem('filtered', JSON.stringify(this.navItems));
    localStorage.setItem('landingurl', this.navItems[0].children[0].url);
  }
}
