﻿
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


//- tato trida umi v zadanem textu najit uplne vsechny palindromy (i kdyby se vzajemne prekryvaly)
//- minimalni delku hledanych palindromu je treba zadat
//- ignoruje rozdily malych a velkych pismen
//- rozpoznava i znaky s diakritikou (takze napr. 'c' je neco jineho nez 'č'...)
//- sleduje pouze pismena (cislice ignoruje, stejne jako interpunkci a dalsi zvlastni znaky)


public class Palindromy
{

    public string origVstup;
    public int minimalniDelkaPalindromu;
    public int pocetPalindromu = 0;
    public string vstup;
    public HashSet<string> palindromy;


    public static void Main(string[] args)
    {

        Palindromy p = new Palindromy();
        bool zadanaDelka;

        Console.WriteLine("Tento program nalezne palindromy minimální požadované délky v zadaném textu.\nMezery, interpunkce a číslice nejsou brány v úvahu. Jsou rozlišována písmena s diakritikou a bez diakritiky, stejně jako i/y, naopak použití velkých nebo malých písmeen nehraje roli.\n");

        do
        {
            Console.WriteLine("Zadejte požadovanou minimální délku palindromu (min. 3 znaky):");
            string minimalniDelkaPalindromuString;
            minimalniDelkaPalindromuString = Console.ReadLine();
            zadanaDelka = Int32.TryParse(minimalniDelkaPalindromuString, out p.minimalniDelkaPalindromu);
            if ((!zadanaDelka) || p.minimalniDelkaPalindromu < 3)
            {
                Console.WriteLine("Prosím zadejte celé číslo s minimální hodnotou 3.");
            }
        } while ((!zadanaDelka) || p.minimalniDelkaPalindromu < 3);


        Console.WriteLine("\nZadejte text:");
        string zadanyText = Console.ReadLine();
        p.NajdiPalindromy(zadanyText);


        Console.WriteLine("\nNalezené palindromy:");
        p.VypisNalezenePalindromy();

    }


    public HashSet<string> NajdiPalindromy(string obdrzenyVstup)
    {
        palindromy = new HashSet<string>();

        if (obdrzenyVstup.Length < minimalniDelkaPalindromu)
        {
            return palindromy;  // retezec kratsi nez je minimalniDelkaPalindromu nemuze obsahovat zadny palindrom => vratime prazdny seznam
        }

        string nalezenyPalindrom = null;
        origVstup = obdrzenyVstup;
        vstup = obdrzenyVstup.ToUpper();

        // pro kazdy znak v retezci budeme zjistovat, jestli neni stredem palindromu
        // staci testovat indexy znaku od 1 do n-1, protoze stred palindromu muze byt nejdriv na indexu 1 (tj. druhy znak), nejpozdeji na predposlednim znaku (n-1)


        for (int i = 1; i < vstup.Length - 1; i++)
        {

            // Pokud na danem indexu neni pismeno, muzeme hledani palindromu od tohoto indexu rovnou preskocit

            if (!Char.IsLetter(vstup[i]))
            {
                continue;  // dany znak neni pismeno => pokracuj dalsi iteraci (tj. testovanim dalsiho znaku v retezci)
            }

            // faze 1 - hledej palindrom s prostrednim znakem (liche delky)
            nalezenyPalindrom = CheckPalindromSProstrednimZnakem(i);


            if (nalezenyPalindrom != null && CistaDelkaPalindromu(nalezenyPalindrom) >= minimalniDelkaPalindromu)
            { // nasel se palindrom
                palindromy.Add(nalezenyPalindrom);
            }

            // faze 2 - hledej palindrom sude delky (maji uprostred dva stejne znaky)
            nalezenyPalindrom = CheckPalindromBezProstrednihoZnaku(i);
            if (nalezenyPalindrom != null && CistaDelkaPalindromu(nalezenyPalindrom) >= minimalniDelkaPalindromu)
            { // nasel se palindrom
                palindromy.Add(nalezenyPalindrom);
            }
        }

        return palindromy;
    }


    private int CistaDelkaPalindromu(string inputPalindrom)
    {
        string nalezenyPalindromBezMezer = Regex.Replace(inputPalindrom, "[^a-zA-ZáéíóúůýěščřžďťňÁÉÍÓÚŮÝĚŠČŘŽĎŤŇ]", "");
        return nalezenyPalindromBezMezer.Length;
    }


    private String CheckPalindromSProstrednimZnakem(int indexStreduPalindromu) //hledani palindromu liche delky
    {
        int? pocatecniIndexPalindromu = indexStreduPalindromu;
        int? koncovyIndexPalindromu = indexStreduPalindromu;

        return CheckPalindrom(pocatecniIndexPalindromu, koncovyIndexPalindromu);
    }



    private String CheckPalindromBezProstrednihoZnaku(int indexStreduPalindromu)  //hledani palindromu sude delky
    {
        int pocatecniIndexPalindromu = indexStreduPalindromu;
        int? koncovyIndexPalindromu = VratPoziciNasledujicichoPismene(indexStreduPalindromu);
        if (koncovyIndexPalindromu == null)
        { // k danemu znaku se nepodarilo najit zadne nasledujici pismeno 
            return null;
        }

        // pismeno na danem indexu neni stejne jako pismeno nasledujici, takze se nemuze jednat o palindrom
        if (vstup[pocatecniIndexPalindromu] != vstup[(int)koncovyIndexPalindromu])
        {
            return null;
        }

        return CheckPalindrom(pocatecniIndexPalindromu, koncovyIndexPalindromu);
    }


    private String CheckPalindrom(int? pocatecniIndexPalindromu, int? koncovyIndexPalindromu)
    {
        bool jePalindrom = false;

        int? indexPredchozihoZnaku = VratPoziciPredchozihoPismene((int)pocatecniIndexPalindromu);
        int? indexNasledujicihoZnaku = VratPoziciNasledujicichoPismene((int)koncovyIndexPalindromu);

        while (indexPredchozihoZnaku != null && indexNasledujicihoZnaku != null && vstup[(int)indexPredchozihoZnaku] == vstup[(int)indexNasledujicihoZnaku])
        {
            jePalindrom = true;
            pocatecniIndexPalindromu = indexPredchozihoZnaku;
            koncovyIndexPalindromu = indexNasledujicihoZnaku;
            indexPredchozihoZnaku = VratPoziciPredchozihoPismene((int)indexPredchozihoZnaku);
            indexNasledujicihoZnaku = VratPoziciNasledujicichoPismene((int)indexNasledujicihoZnaku);
        }

        if (jePalindrom)
        {
            return origVstup.Substring((int)pocatecniIndexPalindromu, (int)koncovyIndexPalindromu - (int)pocatecniIndexPalindromu + 1);
        }
        return null;

    }


    private int? VratPoziciPredchozihoPismene(int index)
    {
        int? indexPredchozihoZnaku = index - 1;

        // pokud se nacte neco jineho nez pismeno, cti dal (pokud nejsme na zacatku retezce)
        while (indexPredchozihoZnaku >= 0 && !Char.IsLetter(vstup[(int)indexPredchozihoZnaku]))
        {
            indexPredchozihoZnaku--;
        }

        // pokud se zadne predchozi pismeno nenaslo (jsme na zacatku retezce nebo se nacetlo neco, co neni pismeno), vrat null, jinak vrat index nalezeneho pismene
        return (indexPredchozihoZnaku < 0) ? null : indexPredchozihoZnaku;
    }


    private int? VratPoziciNasledujicichoPismene(int index)
    {
        int? indexDalsihoZnaku = index + 1;
        int? posledniIndexVstupu = vstup.Length - 1;

        // pokud se nacte neco jineho nez pismeno, cti dal (pokud nejsme na konci retezce)
        while (indexDalsihoZnaku <= posledniIndexVstupu && !Char.IsLetter(vstup[(int)indexDalsihoZnaku]))
        {
            indexDalsihoZnaku++;
        }

        // pokud se nasledujici znak nenasel (jsme na konci retezce nebo se nacetlo neco, co neni pismeno), vrat null, jinak vrat index nalezeneho pismene
        return (indexDalsihoZnaku > posledniIndexVstupu) ? null : indexDalsihoZnaku;
    }



    public void VypisNalezenePalindromy()
    {
        foreach (string palindrom in palindromy)
        {
            Console.WriteLine(palindrom);
            pocetPalindromu++;
        }

        Console.WriteLine($"\nPočet nalezených palindromů: {pocetPalindromu} \n");

    }


}