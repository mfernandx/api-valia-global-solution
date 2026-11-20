using Microsoft.EntityFrameworkCore;
using ValiaApiWeb.Data;
using ValiaApiWeb.Domain.Models;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.Security;
using ValiaApiWeb.Interfaces;
using ValiaApiWeb.Domain.Exceptions;

namespace ValiaApiWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly ValiaDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public AuthService(ValiaDbContext context, IPasswordService passwordService, IJwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        // Realiza o cadastro de um novo usuário
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("O nome completo é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("O e-mail é obrigatório.");

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                throw new ArgumentException("A senha deve possuir pelo menos 8 caracteres.");

            string normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Verifica se o e-mail já está cadastrado
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);

            if (emailExists)
                throw new BusinessConflictException("Este e-mail já está cadastrado.");

            // Gera o hash seguro da senha
            string passwordHash = _passwordService.HashPassword(request.Password);

            // Cria a entidade respeitando seu construtor
            var user = new User(request.FullName, normalizedEmail, passwordHash, request.PhoneNumber);

            // Persiste o usuário no banco de dados
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Gera o JWT após a persistência
            return CreateAuthResponse(user);
        }

        // Realiza a autenticação de um usuário existente
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");
            }

            string normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Consulta o usuário pelo e-mail
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            // Não revela se o e-mail existe
            if (user == null)
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

            // Verifica a senha informada
            bool validPassword = _passwordService.VerifyPassword(user.PasswordHash,request.Password);

            if (!validPassword)
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

            // Impede a autenticação de contas desativadas
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Não foi possível autenticar esta conta.");

            return CreateAuthResponse(user);
        }

        // Monta a resposta sem expor o hash da senha
        private AuthResponse CreateAuthResponse(User user)
        {
            GeneratedToken generatedToken = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Token = generatedToken.Token,
                ExpiresAt = generatedToken.ExpiresAt
            };
        }
    }
}

