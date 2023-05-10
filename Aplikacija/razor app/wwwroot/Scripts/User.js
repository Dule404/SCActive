export class User{
    constructor(id,name,surname,email,number,role) {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.email=email;
        this.number=number;
        this.role = role; // 0 clan 1 trener 2 personalni
    }
}