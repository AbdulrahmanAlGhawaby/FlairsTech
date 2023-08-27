import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { HelperService } from '../helper/helper.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private helper: HelperService) {

  }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {

    if (sessionStorage.getItem('token') || this.helper.getCookie('token')) {
      if (!sessionStorage.getItem('token'))
        sessionStorage.setItem('token', this.helper.getCookie('token'));
      return true;
    }

    else {
      this.router.navigate(['/login']);
      return false;
    }
  }

  getCookie(name: string) {
    var name = name + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      c = c.trim();
      if (c.indexOf(name) == 0) {
        var cookieValue = c.substring(name.length, c.length);
        var _encodedCookie = decodeURIComponent(cookieValue);
        return _encodedCookie;
      }
    }
    return '';
  }
}
