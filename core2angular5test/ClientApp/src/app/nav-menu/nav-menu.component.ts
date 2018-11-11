import { Component } from '@angular/core';
import { AuthService } from "../services/auth.service";
import { Router } from "@angular/router";

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.less']
})

export class NavMenuComponent {
    isExpanded = false;

    constructor(
        public auth: AuthService,//public properties can be used in template file
        private router: Router)
    {

    }

    collapse() {
        this.isExpanded = false;
    }

    toggle() {
        this.isExpanded = !this.isExpanded;
    }

    logout(): boolean {
        // logs out the user, then redirects him to Home View.
        if (this.auth.logout()) {
            this.router.navigate([""]);
        }
        return false;
    }
}
