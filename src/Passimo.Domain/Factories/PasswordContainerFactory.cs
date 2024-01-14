using Passimo.Domain.Model;

namespace Passimo.Domain.Factories;

public abstract class PasswordContainerFactory<T> : IPasswordContainerFactory<T> where T : IPasswordContainer, new()
{
    public virtual T Create(string? name)
    {
        var passwordContainer = new T();
        if (!string.IsNullOrEmpty(name)) passwordContainer.Name = name;
        return passwordContainer;
    }
}

public class PasswordEntryFactory : PasswordContainerFactory<PasswordEntry>, IPasswordEntryFactory
{
    private readonly IPasswordEntryFieldFactory _fieldFactory;

    public PasswordEntryFactory(IPasswordEntryFieldFactory fieldFactory)
    {
        _fieldFactory = fieldFactory;
    }

    public override PasswordEntry Create(string? name)
    {
        var entry = base.Create(name);
        entry.Fields.Add(_fieldFactory.CreateDescription());
        entry.Fields.Add(_fieldFactory.CreateUsername());
        entry.Fields.Add(_fieldFactory.CreateEmail());
        entry.Fields.Add(_fieldFactory.CreateUrl());
        entry.Fields.Add(_fieldFactory.CreatePassword());

        return entry;
    }
}

public class PasswordGroupFactory : PasswordContainerFactory<PasswordGroup>, IPasswordGroupFactory
{
    private readonly IPasswordEntryFieldFactory _fieldFactory;

    public PasswordGroupFactory(IPasswordEntryFieldFactory fieldFactory)
    {
        _fieldFactory = fieldFactory;
    }

    public override PasswordGroup Create(string? name)
    {
        var entry = base.Create(name);
        entry.Fields.Add(_fieldFactory.CreateDescription());
        entry.Fields.Add(_fieldFactory.CreateUsername());
        entry.Fields.Add(_fieldFactory.CreateEmail());
        entry.Fields.Add(_fieldFactory.CreateUrl());

        return entry;
    }
}
