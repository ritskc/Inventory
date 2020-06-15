import { Component, OnDestroy, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { navItems } from './../../_nav';
import { AuthService } from '../../user/auth.service';
import { Router } from '@angular/router';
import { CompanyService } from '../../company/company.service';


@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent implements OnDestroy {
  public clonedNavItems = JSON.parse(localStorage.getItem('filtered')); //JSON.parse(JSON.stringify(navItems));
  public sidebarMinimized = true;
  private changes: MutationObserver;
  public element: HTMLElement;
  companyId: number = 1;
  userAvatar: string = '';
  
  constructor(private authService: AuthService, private router: Router,
              private companyService: CompanyService, @Inject(DOCUMENT) _document?: any) {

    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = _document.body.classList.contains('sidebar-minimized');
    });
    this.element = _document.body;
    this.changes.observe(<Element>this.element, {
      attributes: true,
      attributeFilter: ['class']
    });

    if (localStorage.getItem('username')) {
      this.userAvatar = localStorage.getItem('username').substr(0, 1).toUpperCase();
    }

    if (localStorage.getItem('currentCompany'))
      this.companyId = parseInt(localStorage.getItem('currentCompany'));
  }

  setCompany(event) {
    if (event.target.value == 1) {
      this.companyService.setHarisons();
    } else if (event.target.value == 2) {
      this.companyService.setCastAndForge();
    } else {
      this.companyService.setTest();
    }
    var landingUrl = localStorage.getItem('landingurl')
    this.router.navigateByUrl(landingUrl);
    window.location.reload();
  }

  setHarrisons() {
    this.companyService.setHarisons();
  }

  setCastAndForge() {
    this.companyService.setCastAndForge();
  }

  ngOnDestroy(): void {
    this.changes.disconnect();
  }

  logout() {
    localStorage.clear();
    this.authService.logout();
    this.router.navigate(['/']);
  }
}
