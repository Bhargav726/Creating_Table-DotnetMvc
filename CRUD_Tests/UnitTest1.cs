namespace CRUD_Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        //Arrange
        MyMath m=new MyMath();
        int a=10, b=20;
        int expect=30; 
        //Act
        int actual=m.add(a,b);
        //Assert
        Assert.Equal(expect,actual);
    }
}
