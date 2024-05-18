using CleanArchitecture.Application.Abstractions.Messaging;
using CleanArchitecture.Domain.Abstractions;
using CleanArchitecture.Domain.Users;

namespace CleanArchitecture.Application.Users.RegisterUser
{
    internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // validar que usuario no exista por email
            var email = new Email(request.Email);
            var userExists = await _userRepository.IsUserExists(email);

            if (userExists)
            {
                return Result.Failure<Guid>(UserErrors.AlreadyExists);
            }

            // encriptar password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // crear objeto user
            var user = User.Create(
                    new Nombre(request.Nombre),
                    new Apellido(request.Apellidos),
                    new Email(request.Email),
                    new PasswordHash(passwordHash));

            // insertar el usuario
            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

            return user.Id!.Value;
        }
    }
}
