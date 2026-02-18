import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule, Router } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  authService = inject(AuthService);
  router = inject(Router);
  usuario$ = this.authService.estaLogueado$.pipe(
    map(isLoggedIn => isLoggedIn ? this.authService.obtenerUsuarioAlmacenado() : null)
  );

  constructor() { }

  cerrarSesion() {
    this.authService.cerrarSesion().subscribe(() => {
      this.router.navigate(['/login']);
    });
  }
}
