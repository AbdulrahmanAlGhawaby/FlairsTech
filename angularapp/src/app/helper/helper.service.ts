import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class HelperService {

  constructor() { }

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
