import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router, RouterModule } from '@angular/router';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule],
    template: `
    <div class="d-flex align-items-center justify-content-center min-vh-100 bg-light">
      <div class="card shadow-lg" style="width: 400px;">
        <div class="card-header bg-success text-white text-center py-3">
          <h4 class="mb-0">Registrarse</h4>
        </div>
        <div class="card-body p-4">
          <div *ngIf="errorMessage" class="alert alert-danger alert-dismissible fade show" role="alert">
            {{ errorMessage }}
            <button type="button" class="btn-close" (click)="errorMessage = ''"></button>
          </div>
          
          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
            <div class="mb-3">
              <label for="email" class="form-label">Correo Electrónico</label>
              <div class="input-group">
                <span class="input-group-text"><i class="bi bi-envelope-fill"></i></span>
                <input type="email" id="email" formControlName="email" class="form-control" placeholder="nuevo@usuario.com" 
                  [class.is-invalid]="f['email'].invalid && (f['email'].dirty || f['email'].touched)">
              </div>
              <div *ngIf="f['email'].invalid && (f['email'].dirty || f['email'].touched)" class="invalid-feedback d-block">
                Correo inválido.
              </div>
            </div>

            <div class="mb-3">
              <label for="password" class="form-label">Contraseña</label>
              <div class="input-group">
                <span class="input-group-text"><i class="bi bi-lock-fill"></i></span>
                <input type="password" id="password" formControlName="password" class="form-control" placeholder="******"
                  [class.is-invalid]="f['password'].invalid && (f['password'].dirty || f['password'].touched)">
              </div>
              <div *ngIf="f['password'].invalid && (f['password'].dirty || f['password'].touched)" class="invalid-feedback d-block">
                Contraseña requerida (min 6 caracteres).
              </div>
            </div>

            <div class="d-grid mt-4">
              <button type="submit" class="btn btn-success btn-lg" [disabled]="registerForm.invalid || isLoading">
                <span *ngIf="isLoading" class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                {{ isLoading ? 'Registrando...' : 'Registrar' }}
              </button>
            </div>
          </form>
        </div>
        <div class="card-footer text-center text-muted py-3">
          <small>¿Ya tienes cuenta? <a routerLink="/login">Inicia Sesión</a></small>
        </div>
      </div>
    </div>
  `,
    styles: [`
    .min-vh-100 { min-height: 100vh; }
    .bg-light { background-color: #f8f9fa !important; }
  `]
})
export class RegisterComponent {
    registerForm: FormGroup;
    errorMessage: string = '';
    isLoading: boolean = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.registerForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    get f() { return this.registerForm.controls; }

    onSubmit() {
        if (this.registerForm.invalid) return;

        this.isLoading = true;
        this.errorMessage = '';

        this.authService.registrarUsuario(this.registerForm.value).subscribe({
            next: () => {
                this.isLoading = false;
                alert('Usuario registrado con éxito. Ahora puedes iniciar sesión.');
                this.router.navigate(['/login']);
            },
            error: (err: any) => {
                this.isLoading = false;
                if (err.error && err.error.message) {
                    this.errorMessage = err.error.message;
                } else {
                    this.errorMessage = 'Error al registrar usuario.';
                }
            }
        });
    }
}
