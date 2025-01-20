import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';
import { NavbarComponent } from '../../../../core/component/navbar/navbar.component';
import { FooterComponent } from '../../../../core/component/footer/footer.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavbarComponent, FooterComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {
    // Định nghĩa form group với custom validator
    this.registerForm = this.fb.group(
      {
        fullName: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        phoneNumber: ['', [Validators.required, Validators.pattern(/^0\d{9}$/)]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator } // Áp dụng custom validator
    );
  }

  // Custom validator kiểm tra mật khẩu và xác nhận mật khẩu
  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  onRegister() {
    if (this.registerForm.valid) {
      const formData = this.registerForm.value;

      // Gửi yêu cầu đến API
      this.authService.registerUser(formData).subscribe({
        next: (response) => {
          console.log('Đăng ký thành công:', response);
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Đăng ký thất bại:', err);
          this.errorMessage = err.error?.message || 'Đăng ký thất bại. Vui lòng thử lại!';
        },
      });
    } else {
      this.errorMessage = 'Vui lòng điền đầy đủ thông tin!';
    }
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }
}
